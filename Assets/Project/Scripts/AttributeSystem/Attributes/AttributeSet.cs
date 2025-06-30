using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using SaintsField;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common;
using Project.Scripts.Common.GameplayTags;
using Project.Scripts.Common.Input;
using Project.Scripts.Util.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.AttributeSystem.Attributes;

[DisallowMultipleComponent]
public class AttributeSet : MonoBehaviour, IAttributeReader, IPresentable, IPlayerControllable {
    public static event UnityAction<AttributeSet> OnOpen = delegate { };
    
    private bool IsInitialised { get; set; }
    public Guid Id { get; private set; }
    
    [field: SerializeField, ReadOnly] public string Identifier { get; set; }
    [NotNull] [field: SerializeField] public AttributeDefinition? Source { get; private set; }
    [field: SerializeField] private AttributeConversion? Converter { get; set; }
    public Dictionary<string, AttributeType> Defined { get; private init; } = [];
    private Dictionary<string, string> MaxAttributes { get; init; } = [];
    private Dictionary<string, int> HardLimits { get; init; } = [];
    private ModifierManager Mediator { get; init; } = new ModifierManager();
    private Queue<(Modifier modifier, int remaining)> RecurringModifiers { get; init; } = [];
    private Queue<(int remaining, Action onExpire)> TimedModifiers { get; init; } = [];
    private float NextUpdateTime { get; set; }
    private bool IsFrozen { get; set; }
    
    public AdvancedDropdownList<string> AllAccessibleAttributes => this.Source.AllTags();

    public event UnityAction<AttributeChange> OnAttributeChanged = delegate { };

    private AttributeSet() {
        this.Id = Guid.NewGuid();
        this.Identifier = this.Id.ToString();
    }

    private void Awake() {
        this.Id = Guid.Parse(this.Identifier);
    }

    private void Start() {
        GameEvents.OnPause += () => this.IsFrozen = true;
        GameEvents.OnPlay += () => this.IsFrozen = false;
        PreorderIterator<AttributeType> iterator = new PreorderIterator<AttributeType>(this.Source.Nodes);
        iterator.ForEach = attribute => this.Defined.Add(attribute.Name, attribute);
        this.Source.Traverse(iterator);
        this.NextUpdateTime = Time.time + 1;
    }

    public void Initialise(IEnumerable<AttributeInitialisationData> initial, params GameplayEffect[] effects) {
        if (this.IsInitialised) {
            Logging.Error("The attribute set is already initialised.", this);
            return;   
        }
        
        this.IsInitialised = true;
        this.Defined.Where(pair => pair.Value.IsLeaf).ForEach(init);
        initial.ForEach(data => this.AddModifier(data.ModifierForm));
        return;

        void init(KeyValuePair<string, AttributeType> pair) {
            switch (pair.Value.HowToClamp) {
                case AttributeType.ClampPolicy.CapByAttribute:
                    this.MaxAttributes.Add(pair.Key, pair.Value.Cap);
                    break;
                case AttributeType.ClampPolicy.CapByValue:
                    this.HardLimits.Add(pair.Key, pair.Value.MaxValue);
                    break;
            }
        }
    }

    private void UpdateAttribute(Modifier modifier, float potential) {
        string key = modifier.Target;
        this.PreAttributeUpdate(key);
        int oldBase = this.ReadBase(key);
        int oldCurrent = this.ReadCurrent(key);
        this.Mediator.Add(modifier);
        int max = this.ReadMax(key);
        float updated = max >= 0 ? Mathf.Clamp(potential, 0, max) : Mathf.Max(potential, 0);
        if (!Mathf.Approximately(updated, potential)) {
            this.Mediator.Add(Modifier.Once(updated - potential, key, ModifierType.FinalOffset));
        }
        
        this.PostAttributeUpdate(key);
        int newBase = this.ReadBase(key);
        int newCurrent = this.ReadCurrent(key);
        this.OnAttributeChanged.Invoke(new AttributeChange(key, oldBase, newBase, oldCurrent, newCurrent));
    }

    private bool TryConvert(float value, string key, out (string key, float value) result) {
        if (!this.Converter) {
            result = default;
            return false;
        }
        
        if (this.Converter.TryConvert(value, key, out result)) {
            return true;
        }
        
        Logging.Warn($"{key} is undefined in {this.gameObject.name}. Check spelling.", this);
        result = default;
        return false;
    }

    private void AddModifier(Modifier modifier) {
        if (!this.Defined.TryGetValue(modifier.Target, out AttributeType? type)) {
            if (!this.TryConvert(modifier.Value, modifier.Target, out (string key, float value) result) ||
                !this.Defined.ContainsKey(result.key)) {
                Logging.Error($"{modifier.Target} is undefined in {this.gameObject.name}", this);
                return;
            }
            
            this.AddModifier(modifier with { Key = modifier.Key with { Target = result.key }, Value = result.value });
            return;
        }

        if (type.BehaveLikeHealth && modifier.Type != ModifierType.FinalOffset) {
            Logging.Warn("You probably should not apply base offset modifiers to a health-like attribute," +
                         " unless for initialisation", this);
        }

        if (!this.Defined[modifier.Target].BehaveLikeHealth && modifier.Type == ModifierType.FinalOffset) {
            Logging.Warn("You probably should not apply final offset modifiers to a non-health-like attribute.", this);
        }

        if (modifier.Duration != 0) {
            foreach (Modifier reduced in modifier.Reduce(this.Defined)) {
                if (reduced.Type == ModifierType.FinalOffset) {
                    this.RecurringModifiers.Enqueue((reduced, modifier.Duration));
                } else {
                    this.AddSimpleModifier(reduced);
                    if (modifier.Duration > 0) {
                        this.TimedModifiers.Enqueue((modifier.Duration, () => this.Mediator.Remove(reduced)));
                    }
                }
            }
        } else {
            modifier.Reduce(this.Defined).ForEach(this.AddSimpleModifier);
        }
    }

    private void AddSimpleModifier(Modifier modifier) {
        float potential = this.Mediator.Project(modifier.Target, modifier);
        if (!this.Defined[modifier.Target].AllowNegative && this.ReadCurrent(modifier.Target) == 0 && potential <= 0) {
            // E.g. if -20 fire resistance does not turn 0 fire resistance back to a positive value,
            // we should just re-direct the modifier to +20 fire weakness.
            if (this.TryConvert(modifier.Value, modifier.Target, out (string key, float value) res)) {
                this.AddModifier(modifier with { Key = modifier.Key with { Target = res.key }, Value = res.value });
                return;
            }
                
            Logging.Warn($"Failed to add modifier {modifier}. Probably you missed some edge cases.", this);
            return;
        }
            
        this.UpdateAttribute(modifier, potential);
    }

    public void AddEffect(
        GameplayEffect effect, GameObject instigator, GameplayEffectExecutionArgs? args = null,
        Action<GameObject>? onComplete = null, Action<GameObject>? onSuccess = null, Action<GameObject>? onFail = null
    ) {
        if (!instigator.TryGetComponent(out IAttributeReader source)) {
            this.AddEffect(effect, args, onComplete, onSuccess, onFail);
        } else {
            this.AddEffect(effect, source, args, onComplete, onSuccess, onFail);
        }
    }

    public void AddEffect(
        GameplayEffect effect, IAttributeReader instigator, GameplayEffectExecutionArgs? args = null,
        Action<GameObject>? onComplete = null, Action<GameObject>? onSuccess = null, Action<GameObject>? onFail = null
    ) {
        args ??= GameplayEffectExecutionArgs.Builder.From(instigator).Build();
        switch (effect.Execute(this, args, out IEnumerable<Modifier> outcome)) {
            case GameplayEffectExecutionResult.Success:
                outcome.ForEach(this.AddModifier);
                onComplete?.Invoke(this.gameObject);
                onSuccess?.Invoke(this.gameObject);
                break;
            case GameplayEffectExecutionResult.Failure:
                onComplete?.Invoke(this.gameObject);
                onFail?.Invoke(this.gameObject);
                break;
            case GameplayEffectExecutionResult.Error:
                throw new ArgumentException($"Error executing gameplay effect {effect}");
        }
    }

    public void AddEffect(
        GameplayEffect effect, GameplayEffectExecutionArgs? args = null, Action<GameObject>? onComplete = null,
        Action<GameObject>? onSuccess = null, Action<GameObject>? onFail = null
    ) {
        args ??= GameplayEffectExecutionArgs.Empty;
        switch (effect.Execute(this, args, out IEnumerable<Modifier> outcome)) {
            case GameplayEffectExecutionResult.Success:
                outcome.ForEach(this.AddModifier);
                onComplete?.Invoke(this.gameObject);
                onSuccess?.Invoke(this.gameObject);
                break;
            case GameplayEffectExecutionResult.Failure:
                onComplete?.Invoke(this.gameObject);
                onFail?.Invoke(this.gameObject);
                break;
            case GameplayEffectExecutionResult.Error:
                throw new ArgumentException($"Error executing gameplay effect {effect}");
        }
    }

    /// <summary>
    /// Prepare an attribute update.
    /// </summary>
    /// <param name="updating">The key of the attribute to be updated.
    /// The current attribute data is not yet replaced with this new data.</param>
    protected virtual void PreAttributeUpdate(string updating) { }

    /// <summary>
    /// Perform any actions after an attribute has been updated.
    /// </summary>
    /// <param name="updated">The updated attribute key. It is the current attribute data.</param>
    protected virtual void PostAttributeUpdate(string updated) { }

    public Attribute Read(string attribute) {
        return new Attribute(attribute, this.ReadCurrent(attribute)) {
            BaseValue = this.ReadBase(attribute), MaxValue = this.ReadMax(attribute)
        };
    }

    public int ReadCurrent(string attribute) {
        if (this.Defined[attribute].AllowNegative) {
            return Mathf.CeilToInt(this.Mediator.Query(attribute, 0,
                ModifierManager.QueryFlag.AllowNegativeResult |
                ModifierManager.QueryFlag.FlipMultiplierSignOnNegativeBase));
        }
        
        return Mathf.CeilToInt(this.Mediator.Query(attribute, 0));
    }

    public int ReadBase(string attribute) {
        return Mathf.CeilToInt(this.Mediator.ModifierMagnitude(ModifierType.BaseOffset, attribute));
    }

    public int ReadMax(string attribute) {
        int max = this.MaxAttributes.TryGetValue(attribute, out string cap) ? this.ReadCurrent(cap) : int.MaxValue;
        int limit = this.HardLimits.GetValueOrDefault(attribute, int.MaxValue);
        int effective = Mathf.Min(max, limit);
        return effective == int.MaxValue ? -1 : effective;
    }

    private void LateUpdate() {
        if (Time.time < this.NextUpdateTime) {
            return;
        }

        if (this.IsFrozen) {
            this.NextUpdateTime = Time.time + 1;
            return;
        }

        int count = this.RecurringModifiers.Count;
        for (int i = 0; i < count; i += 1) {
            (Modifier modifier, int remaining) = this.RecurringModifiers.Dequeue();
            this.AddSimpleModifier(modifier);
            if (remaining < 0) {
                this.RecurringModifiers.Enqueue((modifier, remaining));
            } else {
                remaining -= 1;
                if (remaining > 0) {
                    this.RecurringModifiers.Enqueue((modifier, remaining));
                }
            }
        }

        count = this.TimedModifiers.Count;
        for (int i = 0; i < count; i += 1) {
            (int remaining, Action onExpire) = this.TimedModifiers.Dequeue();
            if (remaining < 0) {
                this.TimedModifiers.Enqueue((remaining, onExpire));
            } else {
                remaining -= 1;
                if (remaining > 0) {
                    this.TimedModifiers.Enqueue((remaining, onExpire));
                } else {
                    onExpire.Invoke();
                }
            }
        }

        this.NextUpdateTime = Time.time + 1;
    }

    public string FormatAsText() {
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<string, AttributeType> entry in this.Defined) {
            if (!entry.Value.IsLeaf) {
                sb.AppendLine();
                continue;
            }
            
            int current = this.ReadCurrent(entry.Key);
            sb.AppendLine(entry.Value.HowToClamp == AttributeType.ClampPolicy.None
                    ? $"{entry.Value}: {this.ReadCurrent(entry.Key)}"
                    : $"{entry.Value}: {this.ReadCurrent(entry.Key)} / {this.ReadMax(entry.Key)}");
        }
        
        return sb.ToString();
    }

    public void BindInput(InputActions actions) {
        actions.Player.OpenCharacterPanel.performed += _ => AttributeSet.OnOpen.Invoke(this);
    }
}
