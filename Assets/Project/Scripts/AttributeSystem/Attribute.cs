using Project.Scripts.AttributeSystem.AttributeTypes;

namespace Project.Scripts.AttributeSystem;

public readonly record struct Attribute(
    AttributeType Type,
    int BaseValue,
    int CurrentValue,
    AttributeType Cap = AttributeType.None
) {
    public static Attribute Zero(AttributeType type, AttributeType cap = AttributeType.None) {
        return new Attribute(type, 0, 0, cap);
    }
    
    public override string ToString() {
        return $"{this.Type}: {this.CurrentValue}";
    }
}
