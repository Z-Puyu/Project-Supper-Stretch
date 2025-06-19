using System;
using Project.Scripts.AttributeSystem.Attributes.Definitions;

namespace Project.Scripts.AttributeSystem.Attributes;

[Serializable]
public readonly record struct Attribute(AttributeKey Type, int BaseValue, int CurrentValue) {
    public string Cap { get; private init; } = string.Empty;
    public int HardLimit { get; private init; } = -1;

    public static Attribute WithMaxAttribute(AttributeKey type, int value, AttributeKey cap) {
        return new Attribute(type, value, value) { Cap = cap.FullName };
    }

    public static Attribute WithMaxValue(AttributeKey type, int value, int max) {
        return new Attribute(type, value, value) { HardLimit = max };
    }
    
    public override string ToString() {
        return $"{this.Type}: {this.CurrentValue}";
    }
}
