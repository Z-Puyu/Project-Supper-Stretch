using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.AttributeTypes;
using Project.Scripts.Util.Visitor;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.AttributeSystem.Modifiers;

[DisallowMultipleComponent]
[RequireComponent(typeof(AttributeSet))]
public sealed class ModifierManager : MonoBehaviour, IVisitable<ModifierManager> {
    [NotNull]
    private AttributeSet? AttributeSet { get; set; }
    
    private Dictionary<AttributeType, Vector4> Modifiers { get; init; } = [];
    public event UnityAction<AttributeType> OnModifierUpdate = delegate { };

    private void Awake() {
        this.AttributeSet = this.GetComponent<AttributeSet>();
    }

    public float Query(AttributeType attribute, int @base) {
        return this.Modifiers.TryGetValue(attribute, out Vector4 modifier) 
                ? ((@base + modifier.x) * (1 + modifier.y / 100) + modifier.z) * (1 + modifier.w / 100)
                : @base;
    }

    public void AddModifier(Modifier modifier) {
        if (!this.Modifiers.TryGetValue(modifier.Target, out Vector4 mod)) {
            this.Modifiers.Add(modifier.Target, Vector4.zero + modifier);
        } else {
            this.Modifiers[modifier.Target] += modifier;
        }
        
        this.OnModifierUpdate.Invoke(modifier.Target);
    }
    
    public void RemoveModifier(Modifier modifier) {
        if (!this.Modifiers.TryGetValue(modifier.Target, out Vector4 mod)) {
            return;
        }

        this.Modifiers[modifier.Target] -= modifier;
        this.OnModifierUpdate.Invoke(modifier.Target);
    }

    public void Accept(IVisitor<ModifierManager> visitor) {
        visitor.Visit(this);
    }
}
