using System;
using System.Collections;
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
using Project.Scripts.Util.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.AttributeSystem.Attributes;

[DisallowMultipleComponent]
public class AttributeSet : MonoBehaviour, IEnumerable<Attribute>, IAttributeReader {
    private bool IsInitialised { get; set; }
    public Guid Id { get; private set; }
    
    [NotNull] [field: SerializeField] public AttributeDefinition? Source { get; private set; }
    [field: SerializeField] private AttributeConversion? Converter { get; set; }
    
    public Dictionary<string, AttributeType> Defined { get; private init; } = [];
    [field: SerializeField, ReadOnly] public string Identifier { get; set; }
    
    
    private Dictionary<string, Attribute> Attributes { get; init; } = [];
    private ModifierManager Mediator { get; init; } = new ModifierManager();
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
        PreorderIterator<AttributeType> iterator = new PreorderIterator<AttributeType>(this.Source.Nodes);
        iterator.ForEach = attribute => this.Defined.Add(attribute.Name, attribute);
        this.Source.Traverse(iterator);
    }

    public void Initialise(IEnumerable<AttributeInitialisationData> initial) {
        if (this.IsInitialised) {
            Logging.Error("The attribute set is already initialised.", this);
            return;   
        }
        
        this.IsInitialised = true;
        initial.Where(data => this.Defined.ContainsKey(data.Key)).ForEach(init);
        this.Defined
            .Where(pair => pair.Value.IsLeaf && !this.Attributes.ContainsKey(pair.Key))
            .ForEach(pair => this.Attributes.Add(pair.Key, pair.Value.Zero));
        return;
        
        void init(AttributeInitialisationData data) {
            Attribute zero = this.Defined[data.Key].Zero;
            if (zero.Cap != string.Empty) {
                if (!this.Attributes.ContainsKey(zero.Cap)) {
                    this.Attributes.Add(zero.Cap, new Attribute(zero.Cap, data.Value));   
                }

                int value = this.Defined[data.Key].AllowNegative
                        ? Mathf.Min(data.Value, this.ReadCurrent(zero.Cap))
                        : Mathf.Clamp(data.Value, 0, this.ReadCurrent(zero.Cap));
                this.Attributes.Add(data.Key, zero with { BaseValue = value, CurrentValue = value });
            } else {
                int value = this.Defined[data.Key].AllowNegative ? data.Value : Mathf.Max(data.Value, 0);
                this.Attributes.Add(data.Key, zero with { BaseValue = value, CurrentValue = value });
            }
        }
    }

    private void UpdateAttribute(string key, int value) {
        if (!this.Defined.TryGetValue(key, out AttributeType def)) {
            throw new ArgumentException($"{key} is undefined in {this.gameObject.name}. Check spelling.");    
        }
        
        Attribute old = this.Read(key);
        int @new = def.AllowNegative ? Mathf.Min(value, this.ReadMax(key)) : Mathf.Clamp(value, 0, this.ReadMax(key));
        Attribute updated = old with { CurrentValue = @new };
        this.Attributes[key] = updated;
        this.PostAttributeUpdate(updated);
        this.OnAttributeChanged.Invoke(new AttributeChange(key, old.BaseValue, updated.BaseValue,
            old.CurrentValue, updated.CurrentValue));
    }

    private void AddModifier(Modifier modifier) {
        if (!this.Defined.ContainsKey(modifier.Target)) {
            if (!this.Converter ||
                !this.Converter.TryConvert(modifier.Value, modifier.Target, out (string key, float value) result) ||
                !this.Defined.ContainsKey(result.key)) {
                Logging.Error($"{modifier.Target} is undefined in {this.gameObject.name}", this);
                return;
            }
            
            this.AddModifier(modifier with { Target = result.key, Value = result.value });
            return;
        }

        foreach (Modifier m in modifier.Reduce(this.Defined)) {
            this.Mediator.Add(m);
            int updated = Mathf.CeilToInt(this.Mediator.Query(modifier.Target, this.ReadBase(modifier.Target)));
            if (updated < 0 && !this.Defined[m.Target].AllowNegative) {
                this.Mediator.Add(Modifier.Of(-updated, modifier.Target, ModifierType.FinalOffset));
                updated = 0;
            }
            
            int max = this.ReadMax(modifier.Target);
            if (updated > max) {
                this.Mediator.Add(Modifier.Of(max - updated, modifier.Target, ModifierType.FinalOffset));
                updated = max;
            }
        
            this.UpdateAttribute(modifier.Target, updated);
        }
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
    /// <param name="newData">The new attribute data.
    /// The current attribute data is not yet replaced with this new data.</param>
    protected virtual void PreAttributeUpdate(Attribute newData) { }

    /// <summary>
    /// Perform any actions after an attribute has been updated.
    /// </summary>
    /// <param name="updated">The updated attribute data. It is the current attribute data.</param>
    protected virtual void PostAttributeUpdate(Attribute updated) { }

    public Attribute Read(string attribute) {
        if (this.Attributes.TryGetValue(attribute, out Attribute value)) {
            return value;
        }

        /*if (!this.Converter) {
            throw new ArgumentException($"{attribute} is undefined in {this.gameObject.name}.");   
        }
        
        foreach ((string key, Attribute data) in this.Attributes) {
            if (!this.Converter.TryConvertTo(key, data.BaseValue, out (string key, float value) res)) {
                continue;
            }
            
            float current = this.Converter.Convert(data.CurrentValue, attribute, key);
            Logging.Warn($"{attribute} is undefined. Returning a converted value.", this);
            return new Attribute(attribute, Mathf.CeilToInt(converted), Mathf.CeilToInt(current));
        }
        */

        throw new ArgumentException($"{attribute} is undefined in {this.gameObject.name}.");
    }

    public int ReadCurrent(string attribute) {
        return this.Read(attribute).CurrentValue;
    }

    public int ReadBase(string attribute) {
        return this.Read(attribute).BaseValue;
    }

    public int ReadMax(string attribute) {
        Attribute value = this.Read(attribute);
        if (value.Cap != string.Empty) {
            return this.ReadCurrent(value.Cap);
        }
        
        return value.HardLimit >= 0 ? value.HardLimit : int.MaxValue;
    }

    public IEnumerator<Attribute> GetEnumerator() {
        return this.Attributes.Values.GetEnumerator();
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder(this.Attributes.Count);
        sb.AppendJoin(' ', this.Attributes.Values);
        return sb.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
}
