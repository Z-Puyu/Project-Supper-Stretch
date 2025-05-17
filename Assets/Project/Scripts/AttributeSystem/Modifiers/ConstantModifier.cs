using System;
using Project.Scripts.AttributeSystem.AttributeTypes;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

[Serializable]
public sealed class ConstantModifier : Modifier {
    [field: SerializeField, Header("Magnitude")]
    private int Value { get; set; }

    public ConstantModifier(AttributeType target, Type valueType, Operation operation, int value)
            : base(target, valueType, operation) {
        this.Value = value;
    }

    protected override float Evaluate() {
        return this.Value;
    }
}