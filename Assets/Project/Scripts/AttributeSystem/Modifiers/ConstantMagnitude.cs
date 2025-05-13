using System;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public sealed class ConstantMagnitude : Magnitude {
    public static ConstantMagnitude Zero { get; } = new ConstantMagnitude(0);
    
    [field: SerializeField]
    private int Value { get; set; }

    public ConstantMagnitude(int value) {
        this.Value = value;
    }

    public override float GetValueWith(AttributeSet? attributes = null) {
        return this.Value;
    }
}
