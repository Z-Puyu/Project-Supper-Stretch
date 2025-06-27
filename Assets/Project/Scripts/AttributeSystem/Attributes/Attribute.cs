namespace Project.Scripts.AttributeSystem.Attributes;

public readonly record struct Attribute(string Type, int CurrentValue) {
    public int BaseValue { get; init; } = CurrentValue;
    public int MaxValue { get; init; } = CurrentValue;
    
    public override string ToString() {
        return $"{this.Type}: {this.CurrentValue}";
    }
}
