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
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Util.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.AttributeSystem.Attributes;

[DisallowMultipleComponent]
public class AttributeSet : MonoBehaviour, IEnumerable<Attribute>, IAttributeReader {
    private bool IsInitialised { get; set; }
    public Guid Id { get; private set; }
    [field: SerializeField, ReadOnly] public string Identifier { get; set; }
    [NotNull] [field: SerializeField] public AttributeDefinition? AttributeDefinition { get; private set; }
    private Dictionary<string, Attribute> Attributes { get; init; } = [];
    private Dictionary<AttributeKey, Vector4> Modifiers { get; init; } = [];

    public AdvancedDropdownList<AttributeKey> AllAccessibleAttributes => this.AttributeDefinition.FetchLeaves();

    public event UnityAction<AttributeChange> OnAttributeChanged = delegate { };

    private AttributeSet() {
        this.Id = Guid.NewGuid();
        this.Identifier = this.Id.ToString();
    }

    private void Awake() {
        this.Id = Guid.Parse(this.Identifier);
    }

    public void Initialise(IEnumerable<AttributeInitialisationData> initial) {
        if (this.IsInitialised) {
            throw new InvalidOperationException($"Trying to initialise {this.gameObject.name} twice");
        }
        
        this.IsInitialised = true;
        initial.ForEach(init);
        this.AttributeDefinition.PreorderTraverse(forEach: setZeroIfAbsent);
        return;

        void setZeroIfAbsent(AttributeTag attribute) {
            if (attribute.SubAttributes.Count > 0 || this.Attributes.ContainsKey(attribute.Key.FullName)) {
                return;
            }

            this.Attributes.Add(attribute.Key.FullName, attribute.Zero);
        }
                
        
        void init(AttributeInitialisationData data) {
            if (!this.AttributeDefinition.Contains(data.Key, out AttributeTag? a) || a is null) {
                throw new ArgumentException($"Attribute {data.Key} is undefined in attribute set of {this.gameObject.name}");
            }
            
            Attribute zero = a.Zero;
            if (zero.Cap != string.Empty) {
                if (!this.AttributeDefinition.Contains(zero.Cap, out AttributeTag? cap)) {
                    throw new ArgumentException($"{data.Key} requires an attribute of type {cap?.Key.FullName}");
                }
                
                if (!this.Attributes.ContainsKey(cap!.Key.FullName)) {
                    this.Attributes.Add(cap.Key.FullName, new Attribute(cap.Key.FullName, data.Value, data.Value));   
                }

                int value = Mathf.Clamp(data.Value, 0, this.ReadCurrent(cap.Key.FullName));
                this.Attributes.Add(data.Key.FullName, zero with { BaseValue = value, CurrentValue = value });
            } else {
                this.Attributes.Add(data.Key.FullName, zero with { BaseValue = data.Value, CurrentValue = data.Value });
            }
        }
    }

    /// <summary>
    /// Update the current value of the attribute. Called after every modifier change.
    /// </summary>
    /// <param name="attribute">The attribute to recompute.</param>
    private void Recompute(string attribute) {
        AttributeKey key = attribute;
        int value = this.ReadBase(attribute);
        if (this.Modifiers.TryGetValue(key, out Vector4 m)) {
            value = Mathf.CeilToInt(((value + m.x) * (1 + m.y / 100) + m.z) * (1 + m.w / 100));
        }
        
        this.UpdateAttribute(key, value);
    }

    private void UpdateAttribute(AttributeKey key, int value) {
        if (!this.AttributeDefinition.Contains(key, out AttributeTag? attribute)) {
            throw new ArgumentException($"{key} is undefined in {this.gameObject.name}. Check spelling.");    
        }
        
        write(key);
        foreach (AttributeKey k in attribute!.SameSetSynonyms) {
            write(k);
        }

        return;
        
        void write(AttributeKey k) {
            Attribute old = this.Read(k.FullName);
            Attribute updated = old with { CurrentValue = Mathf.Clamp(value, 0, this.ReadMax(k.FullName)) };
            this.Attributes[k.FullName] = updated;
            this.PostAttributeUpdate(updated);
            this.OnAttributeChanged.Invoke(new AttributeChange(k, old.BaseValue, updated.BaseValue,
                old.CurrentValue, updated.CurrentValue));
        }
    }

    private void AddModifier(Modifier modifier) {
        if (!this.AttributeDefinition.Contains(modifier.Target, out AttributeTag? @as)) {
            throw new ArgumentException($"{modifier.Target} is undefined in {this.gameObject.name}. Check spelling.");
            return;
        }

        if (@as!.SubAttributes.Count > 0) {
            @as.SubAttributes.ForEach(a => this.AddModifier(modifier with { Target = a.Key }));
        }

        AttributeKey key = modifier.Target;
        foreach (AttributeTag attribute in this.AttributeDefinition.CollectIf(isNonzeroAntonym)) {
            modifier = -modifier with { Target = attribute.Key };
            break;
        }

        if (!this.Modifiers.TryAdd(modifier.Target, modifier)) {
            this.Modifiers[modifier.Target] += modifier;
        }

        this.Recompute(modifier.Target.FullName);

        return;
        bool isNonzeroAntonym(AttributeTag a) => a.Antonyms.Contains(key) && this.ReadCurrent(a.Key.FullName) != 0;
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
        if (!this.AttributeDefinition.Contains(attribute, out AttributeTag? @as)) {
            throw new KeyNotFoundException($"{attribute} is undefined in {this.gameObject.name}. Check spelling.");
        }

        if (@as!.SubAttributes.Count > 0) {
            throw new ArgumentException($"{attribute} is a composite attribute.");
        }
        
        return this.Attributes[@as.Key.FullName];
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
