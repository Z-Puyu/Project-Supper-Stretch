using GameplayAbilitiesSystem.Runtime.Attributes;

namespace GameplayAbilitiesSystem.Runtime.Modifiers {
    public readonly record struct Modifier(
        AttributeKey Target,
        ModifierType Type,
        ModifierValue Value
    ) {
        public static Modifier operator -(Modifier modifier) => modifier with { Value = -modifier.Value };
    };
}