using System;
using System.Collections.Generic;
using Project.Scripts.AttributeSystem.AttributeTypes;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public sealed class DynamicallyValuedModifier : Modifier {
    [field: SerializeField, Header("Magnitude")]
    private string ValueLabel { get; set; }
    
    private int Value { get; set; }

    public DynamicallyValuedModifier(AttributeType target, Type valueType, Operation operation, string valueLabel)
            : base(target, valueType, operation) {
        this.ValueLabel = valueLabel;
    }

    private DynamicallyValuedModifier With(int value) {
        return this.ValueLabel == string.Empty
                ? this
                : new DynamicallyValuedModifier(this.Target, this.ValueType, this.OperationType, this.ValueLabel) {
                    Value = value
                };
    }
    
    protected override Modifier With(IReadOnlyDictionary<string, int> magnitudes) {
        return magnitudes.TryGetValue(this.ValueLabel, out int value) ? this.With(value) : base.With(magnitudes);
    }

    protected override Modifier With(string label, int magnitude) {
        return this.ValueLabel == label ? this.With(magnitude) : base.With(label, magnitude);
    }

    protected override float Evaluate() {
        return this.Value;
    }
}