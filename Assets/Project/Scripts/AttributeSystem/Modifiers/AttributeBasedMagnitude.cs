using System;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public sealed class AttributeBasedMagnitude : Magnitude {
    [field: SerializeField] 
    private AttributeType BackingAttribute { get; set; }
    
    [field: SerializeField] 
    private float Coefficient { get; set; } = 1.0f;

    public override float GetValueWith(AttributeSet? attributes = null) {
        return attributes?[this.BackingAttribute].CurrentValue * this.Coefficient ?? 0;
    }
}
