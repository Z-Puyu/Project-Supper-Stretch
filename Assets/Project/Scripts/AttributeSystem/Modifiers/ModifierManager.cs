using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Util.Linq;
using Project.Scripts.Util.Visitor;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[DisallowMultipleComponent]
[RequireComponent(typeof(Attributes.AttributeManagementSystem))]
public sealed class ModifierManager : MonoBehaviour, IVisitable<ModifierManager> {
    [NotNull]
    private Attributes.AttributeManagementSystem? AttributeSystem { get; set; }
    
    private Dictionary<Enum, Vector4> Modifiers { get; init; } = [];

    private void Awake() {
        this.AttributeSystem = this.GetComponent<Attributes.AttributeManagementSystem>();
    }

    public float Query(Enum attribute, int @base) {
        return this.Modifiers.TryGetValue(attribute, out Vector4 modifier) 
                ? ((@base + modifier.x) * (1 + modifier.y / 100) + modifier.z) * (1 + modifier.w / 100)
                : @base;
    }

    public void AddModifier<T, K>(T set, Modifier<K> modifier) where T : Enum where K : Enum {
        if (this.AttributeSystem.FoundAttributeSet(set, out Attributes.AttributeSet attributes)) {
            attributes.AddModifier(modifier);
        }
    }
    
    public void AddModifier<K>(Modifier<K> modifier) where K : Enum {
        this.AttributeSystem.AllSetsUsing<K>().ForEach(set => set.AddModifier(modifier));
    }
    
    public void RemoveModifier<T, K>(T set, Modifier<K> modifier) where T : Enum where K : Enum {
        if (this.AttributeSystem.FoundAttributeSet(set, out Attributes.AttributeSet attributes)) {
            attributes.RemoveModifier(modifier);
        }
    }
    
    public void RemoveModifier<K>(Modifier<K> modifier) where K : Enum {
        this.AttributeSystem.AllSetsUsing<K>().ForEach(set => set.RemoveModifier(modifier));
    }

    public void Accept(IVisitor<ModifierManager> visitor) {
        visitor.Visit(this);
    }
}
