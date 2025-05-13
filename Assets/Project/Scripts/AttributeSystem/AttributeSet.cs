using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Events;
using Project.Scripts.Util.Visitor;
using UnityEngine;

namespace Project.Scripts.AttributeSystem;

public class AttributeSet : MonoBehaviour, IVisitable<AttributeSet> {
    private Dictionary<AttributeType, Attribute> Attributes { get; init; } = [];
    public ModifierManager ModifierManager { get; private init; }
    
    [field: SerializeField] 
    private EventChannel<Attribute>? OnAttributeUpdate { get; set; }

    public AttributeSet() {
        this.ModifierManager = new ModifierManager(this);
    }

    private void Start() {
        this.ModifierManager.OnModifierUpdate += this.Recompute;
    }
    
    public Attribute this[AttributeType type] => this.Attributes.TryGetValue(type, out Attribute value)
            ? value
            : Attribute.Zero(type);

    /// <summary>
    /// Update the current value of the attribute. Called after every modifier change.
    /// </summary>
    /// <param name="attribute">The attribute to recompute.</param>
    private void Recompute(AttributeType attribute) {
        if (!this.Attributes.TryGetValue(attribute, out Attribute data)) {
            data = Attribute.Zero(attribute);
        }
        
        (float baseOffset, float baseMultiplier) = this.ModifierManager.AggregateBaseValueModifiers(attribute);
        (float finalOffset, float finalMultiplier) = this.ModifierManager.AggregateFinalValueModifiers(attribute);
        float final = ((data.BaseValue + baseOffset) * baseMultiplier + finalOffset) * finalMultiplier;
        data = data with { CurrentValue = Mathf.CeilToInt(final) };
        this.PostAttributeUpdate(in data);
    }

    public void SetBaseValue(params AttributeInitialisation[] init) {
        Attribute[] attributes = init.Select(i => new Attribute(i.Type, i.Value, i.Value, i.Cap)).ToArray();
        foreach (Attribute attribute in attributes.Where(a => a.Cap == AttributeType.None)) {
            this.PostAttributeUpdate(in attribute);
        }
        
        foreach (Attribute attribute in attributes.Where(a => a.Cap != AttributeType.None)) {
            this.PostAttributeUpdate(in attribute);
        }
    }

    /// <summary>
    /// Finalise an attribute update. This is the only place where an attribute is updated.
    /// </summary>
    /// <param name="newData">The new attribute data.</param>
    protected virtual void PostAttributeUpdate(in Attribute newData) {
        if (newData.Cap != AttributeType.None) {
            int max = this[newData.Cap].CurrentValue;
            Attribute attribute = newData with { CurrentValue = Mathf.Clamp(newData.CurrentValue, 0, max) };
            this.Attributes[newData.Type] = attribute;
        }
        
        this.OnAttributeUpdate?.Broadcast(this.gameObject, this[newData.Type]);
    }

    public void Accept(IVisitor<AttributeSet> visitor) {
        visitor.Visit(this);
    }
}
