using System;

namespace GameplayAbilities.Modifiers {
    public readonly record struct Modifier(
        ModifierType Type,
        double Value
    ) {
        public static readonly Modifier ZeroShift = new Modifier(ModifierType.Shift, 0);
        public static readonly Modifier ZeroMultiplier = new Modifier(ModifierType.Multiplier, 0);
        public static readonly Modifier ZeroOffset = new Modifier(ModifierType.Offset, 0);
        
        internal int Priority => (int)this.Type + (this.Type == ModifierType.Offset && this.Value < 0 ? 1 : 0);

        public double Modify(double value) {
            return this.Type switch {
                ModifierType.Shift or ModifierType.Offset => value + this.Value,
                ModifierType.Multiplier => value * Math.Max(100 + this.Value, 0) / 100.0f,
                var _ => value
            };
        }
        
        public static Modifier operator -(Modifier modifier) => modifier with { Value = -modifier.Value };

        public static Modifier operator *(Modifier modifier, double multiplier) => modifier with {
            Value = modifier.Value * multiplier
        };
        
        public static Modifier operator +(Modifier modifier, double value) => modifier with {
            Value = modifier.Value + value
        };
    }
}