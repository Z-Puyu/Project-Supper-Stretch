using System;
using GameplayAbilitiesSystem.Runtime.Attributes;

namespace GameplayAbilitiesSystem.Runtime.Modifiers {
    public readonly struct Modifier {
        internal AttributeKey Target { get; }
        internal ModifierType Type { get; }
        internal ModifierValue Value { get; }
        internal Predicate<ModifierEnvironment> Condition { get; }

        internal Modifier(
            AttributeKey target, ModifierType type, ModifierValue value, 
            Predicate<ModifierEnvironment> condition = null
        ) {
            this.Target = target;
            this.Type = type;
            this.Value = value;
            this.Condition = condition;
        }
    }
}