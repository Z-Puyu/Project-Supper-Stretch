using Project.Scripts.AttributeSystem.Attributes.Definitions;

namespace Project.Scripts.AttributeSystem.Attributes;

public readonly record struct AttributeChange(
    string Type,
    int OldBaseValue,
    int NewBaseValue,
    int OldCurrentValue,
    int NewCurrentValue
);
