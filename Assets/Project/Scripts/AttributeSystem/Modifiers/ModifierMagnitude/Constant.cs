using System;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers.ModifierMagnitude;

[Serializable]
public class Constant : Magnitude {
    [field: SerializeField]
    private int Value { get; set; }
    
    public override float Evaluate() {
        return this.Value;
    }

    public override string ToString() {
        return $"{this.Value}";
    }
}
