using System;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public sealed class SetByCallerMagnitude : Magnitude {
    [field: SerializeField]
    public string Label { get; private set; } = "";
    
    public int Value { get; set; }
    
    public override float GetValueWith(AttributeSet? attributes = null) {
        return this.Value;
    }
}