using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public class DynamicMagnitude : Magnitude {
    [field: SerializeField]
    private string ValueLabel { get; set; }
    
    private int Value { get; set; }

    public DynamicMagnitude(string valueLabel, int value) {
        this.ValueLabel = valueLabel;
        this.Value = value;
    }
    
    public override float Evaluate() {
        return this.Value;
    }

    public override float Evaluate(Enum tag) {
        return this.Value;
    }

    public override Magnitude With(string label, int magnitude) {
        return this.ValueLabel != string.Empty && label == this.ValueLabel
                ? new DynamicMagnitude(this.ValueLabel, magnitude)
                : this;
    }
    
    public override Magnitude With(IReadOnlyDictionary<string, int> magnitudes) {
        return this.ValueLabel != string.Empty && magnitudes.TryGetValue(this.ValueLabel, out int value)
                ? new DynamicMagnitude(this.ValueLabel, value)
                : this;
    }
}
