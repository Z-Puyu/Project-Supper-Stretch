using System;
using Project.Scripts.AttributeSystem.AttributeTypes;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public sealed class BackingAttribute {
    [field: SerializeField]
    private AttributeType Attribute { get; set; }

    [field: SerializeField]
    private float Coefficient { get; set; }
    
    public BackingAttribute(AttributeType attribute, float coefficient = 1) {
        this.Attribute = attribute;
        this.Coefficient = coefficient;
    }

    public float EvaluateUsing(AttributeSet attributes) {
        return attributes[this.Attribute].CurrentValue * this.Coefficient;
    }
}
