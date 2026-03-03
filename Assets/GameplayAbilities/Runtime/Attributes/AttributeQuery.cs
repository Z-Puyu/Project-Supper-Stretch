using GameplayAbilities.Modifiers;

namespace GameplayAbilities.Attributes {
    internal readonly ref struct AttributeQuery {
        private IAttributeReader Source { get; }
        internal GameplayAttributeType AttributeType { get; }
        private double BaseValue { get; }

        private Modifier[] Modifiers { get; } = {
            Modifier.ZeroBaseOverride, 
            Modifier.ZeroShift, 
            Modifier.ZeroMultiplier, 
            Modifier.ZeroOffset,
            Modifier.ZeroOffset
        };

        internal AttributeQuery(IAttributeReader source, GameplayAttributeType type, double value = 0) {
            this.Source = source;
            this.AttributeType = type;
            this.BaseValue = value;
        }
        
        internal double Evaluate(out double @base) {
            @base = this.Modifiers[0].Modify(this.BaseValue);
            double result = this.BaseValue;
            foreach (Modifier modifier in this.Modifiers) {
                result = this.AttributeType.Clamp(modifier.Modify(result), this.Source);
            }

            return result;
        }
        
        internal void AddModifier(Modifier modifier) {
            this.Modifiers[modifier.Priority] += modifier.Value;
        }
    }
}