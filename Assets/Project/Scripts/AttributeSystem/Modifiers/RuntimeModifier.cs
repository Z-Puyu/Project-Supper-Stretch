using System;
using Project.Scripts.AttributeSystem.Modifiers.ModifierMagnitude;

namespace Project.Scripts.AttributeSystem.Modifiers;

internal sealed class RuntimeModifier : TaggedModifier<Enum> {
    public RuntimeModifier(string tag, Enum target, ModifierType type, string label) : base(target) {
        this.Tag = tag;
        this.ValueType = type;
        this.Magnitude = new DynamicMagnitude(label, 0);
    }
}
