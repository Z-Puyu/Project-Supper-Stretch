using System;
using Project.Scripts.AttributeSystem.Attributes.Definitions;

namespace Project.Scripts.AttributeSystem.Attributes;

public readonly record struct Attribute(string Type, int CurrentValue) {
    public int BaseValue { get; init; } = CurrentValue;
    public string Cap { get; private init; } = string.Empty;
    public int HardLimit { get; private init; } = -1;

    public static Attribute WithMaxAttribute(string type, int value, string cap) {
        return new Attribute(type, value) { BaseValue = value, Cap = cap };
    }

    public static Attribute WithMaxValue(string type, int value, int max) {
        return new Attribute(type, value) { BaseValue = value, HardLimit = max };
    }
    
    public override string ToString() {
        return $"{this.Type}: {this.CurrentValue}";
    }
}
