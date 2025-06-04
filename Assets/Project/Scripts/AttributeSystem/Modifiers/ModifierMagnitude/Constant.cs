using System;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers.ModifierMagnitude;

[Serializable]
public sealed class Constant : Magnitude {
    [field: SerializeField]
    private float ConstantValue { get; set; }
    
    public override float Value => this.ConstantValue;
    
    public Constant() { }
    
    public Constant(float constantValue) {
        this.ConstantValue = constantValue;
    }

    public override string ToString() {
        return $"{this.ConstantValue}";
    }
}
