using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.Common.UI;

namespace Project.Scripts.AttributeSystem.Attributes;

public readonly record struct AttributeChange(
    AttributeKey Type,
    int OldBaseValue,
    int NewBaseValue,
    int OldCurrentValue,
    int NewCurrentValue
) : IPresentable;
