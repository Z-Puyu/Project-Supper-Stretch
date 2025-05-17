using System;
using Project.Scripts.AttributeSystem.Modifiers.ModifierMagnitude;

namespace Project.Scripts.AttributeSystem.Modifiers;

public sealed class RuntimeModifier : TaggedModifier<Enum, Enum> {
    public RuntimeModifier(Enum tag, Enum target, ModifierType type, string label) : base(tag, target) {
        this.ValueType = type;
        this.Magnitude = new DynamicMagnitude(label, 0);
    }
}
