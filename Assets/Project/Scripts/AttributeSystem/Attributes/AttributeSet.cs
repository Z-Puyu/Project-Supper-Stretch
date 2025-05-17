using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Events;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes;

public abstract class AttributeSet : MonoBehaviour {
    [NotNull]
    protected AttributeManagementSystem? System { get; private set; }
    
    public abstract Type AttributeCategory { get; }
    protected Dictionary<Enum, Attribute> Attributes { get; private init; } = [];
    private Dictionary<Enum, Vector4> Modifiers { get; init; } = [];
    
    public abstract Enum Tag { get; }
    
    [field: SerializeField]
    private EventChannel<Attribute>? OnAttributeUpdate { get; set; }

    /// <summary>
    /// Returns the attribute data with the given type. If the attribute set does not contain this attribute,
    /// return a zero attribute.
    /// </summary>
    /// <param name="key">The attribute to query.</param>
    public Attribute this[Enum key] =>
            this.Attributes.TryGetValue(key, out Attribute value) ? value : Attribute.Zero(key);
    
    private void Awake() {
        this.System = this.GetComponentInParent<AttributeManagementSystem>();
    }
    
    public bool FoundAttribute<K>(K key, out Attribute attribute) where K : Enum {
        if (this.AttributeCategory == typeof(K)) {
            return this.Attributes.TryGetValue(key, out attribute);
        }

        attribute = Attribute.Zero(key);
        return false;
    }
    
    /// <summary>
    /// Update the current value of the attribute. Called after every modifier change.
    /// </summary>
    /// <param name="attribute">The attribute to recompute.</param>
    private void Recompute<K>(K attribute) where K : Enum {
        if (this.AttributeCategory != typeof(K)) {
            return;
        }
        
        if (!this.Attributes.TryGetValue(attribute, out Attribute data)) {
            data = Attribute.Zero(attribute);
        }
        
        if (this.Modifiers.TryGetValue(attribute, out Vector4 m)) {
            float modified = ((data.BaseValue + m.x) * (1 + m.y / 100) + m.z) * (1 + m.w / 100);
            data = data with { CurrentValue = Mathf.CeilToInt(modified) };
        }

        this.UpdateAttribute(in data);
    }
    
    public void AddModifier<K>(Modifier<K> modifier) where K : Enum {
        if (this.AttributeCategory != typeof(K)) {
            return;
        }
        
        if (!this.Modifiers.ContainsKey(modifier.Target)) {
            this.Modifiers.Add(modifier.Target, modifier.ToVector4());
        } else {
            this.Modifiers[modifier.Target] += modifier.ToVector4();
        }
        
        this.Recompute(modifier.Target);
    }
    
    public void RemoveModifier<K>(Modifier<K> modifier) where K : Enum {
        if (this.AttributeCategory != typeof(K)) {
            return;
        }
        
        if (!this.Modifiers.ContainsKey(modifier.Target)) {
            return;
        }

        this.Modifiers[modifier.Target] -= modifier.ToVector4();
        this.Recompute(modifier.Target);
    }

    /// <summary>
    /// Prepare an attribute update.
    /// </summary>
    /// <param name="newData">The new attribute data.
    /// The current attribute data is not yet replaced with this new data.</param>
    protected virtual void PreAttributeUpdate(in Attribute newData) { }
    
    /// <summary>
    /// Finalise an attribute update. This is the only place where an attribute is updated.
    /// </summary>
    /// <param name="newData">The new attribute data.</param>
    private void UpdateAttribute(in Attribute newData) {
        this.PreAttributeUpdate(in newData);
        if (newData.Cap is not null) {
            int max = this[newData.Cap].CurrentValue;
            Attribute attribute = newData with { CurrentValue = Mathf.Clamp(newData.CurrentValue, 0, max) };
            this.Attributes[newData.Type] = attribute;
        }

        Attribute updated = this[newData.Type];
        this.OnAttributeUpdate?.Broadcast(this.gameObject, updated);
        this.PostAttributeUpdate(in updated);
    }

    /// <summary>
    /// Perform any actions after an attribute has been updated.
    /// </summary>
    /// <param name="updated">The updated attribute data. It is the current attribute data.</param>
    protected virtual void PostAttributeUpdate(in Attribute updated) { }
}

/// <summary>
/// Base class for all attribute sets.
/// </summary>
/// <typeparam name="T">The enum type used to tag attribute sets in the same system.</typeparam>
/// <typeparam name="K">The enum type used to represent attribute types.</typeparam>
public abstract class AttributeSet<T, K> : AttributeSet where T : Enum where K : Enum {
    public override Type AttributeCategory => typeof(K);
    
    [field: SerializeField]
    private T AttributeSetTag { get; set; }
    
    public override Enum Tag => this.AttributeSetTag;
    
    protected AttributeSet(T attributeSetTag) {
        this.AttributeSetTag = attributeSetTag;
    }
    
    /// <summary>
    /// Returns the attribute data with the given type. If the attribute set does not contain this attribute,
    /// return a zero attribute.
    /// </summary>
    /// <param name="key">The attribute to query.</param>
    public Attribute this[K key] => this.Attributes.TryGetValue(key, out Attribute value) ? value : Attribute.Zero(key);
}
