using System;

namespace Project.Scripts.AttributeSystem.Attributes;

[Serializable]
public readonly record struct Attribute {
    public Enum Type { get; init; }
    public int BaseValue { get; init; }
    public int CurrentValue { get; init; }
    public Enum? Cap { get; init; }
    public int HardLimit { get; init; }
    
    private Attribute(Enum type, int baseValue, int currentValue, Enum? cap, int hardLimit) {
        this.Type = type;
        this.BaseValue = baseValue;
        this.CurrentValue = currentValue;
        this.Cap = cap;
        this.HardLimit = hardLimit;
    }

    public Attribute(Enum type, int value) : this(type, value, value, null, -1) { }
    
    public Attribute(Enum type, int value, Enum cap) : this(type, value, value, cap, -1) { }
    
    public Attribute(Enum type, int value, int max) : this(type, value, value, null, max) { }

    public static Attribute Zero(Enum type) {
        return new Attribute(type, 0);
    }

    public static Attribute Zero(Enum type, Enum cap) {
        return new Attribute(type, 0, cap);
    }

    public static Attribute Zero(Enum type, int max) {
        return new Attribute(type, 0, max);
    }
    
    public override string ToString() {
        return $"{this.Type}: {this.CurrentValue}";
    }
}
