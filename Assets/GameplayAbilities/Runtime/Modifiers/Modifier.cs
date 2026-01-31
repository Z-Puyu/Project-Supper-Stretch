using GameplayAbilities.Attributes;

namespace GameplayAbilities.Modifiers {
    public readonly record struct Modifier(
        AttributeKey Target,
        ModifierType Type,
        ModifierValue Value
    ) {
        public static Modifier operator -(Modifier modifier) => modifier with { Value = -modifier.Value };

        public static Modifier operator *(Modifier modifier, double multiplier) =>
                modifier with { Value = modifier.Value * multiplier };

        public static Modifier operator *(double multiplier, Modifier modifier) =>
                modifier with { Value = modifier.Value * multiplier };
    }
}