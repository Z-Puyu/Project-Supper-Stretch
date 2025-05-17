using System;
using Project.Scripts.AttributeSystem.AttributeTypes;

namespace Project.Scripts.AttributeSystem;

public readonly record struct Attribute(
    Enum Type,
    int BaseValue,
    int CurrentValue,
    Enum? Cap
) {
    public static Attribute Zero(Enum type, Enum? cap = null) {
        return new Attribute(type, 0, 0, cap);
    }
    
    public override string ToString() {
        return $"{this.Type}: {this.CurrentValue}";
    }
}
