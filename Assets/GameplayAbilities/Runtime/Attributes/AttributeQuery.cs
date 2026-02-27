using System.Linq;
using GameplayAbilities.Modifiers;

namespace GameplayAbilities.Attributes {
    internal readonly ref struct AttributeQuery {
        internal IAttributeReader Source { get; }
        internal GameplayAttributeType AttributeType { get; }
        private double BaseValue { get; }
        
        internal Modifier[] Modifiers { get; } = {
            Modifier.ZeroShift, Modifier.ZeroMultiplier, Modifier.ZeroOffset, Modifier.ZeroOffset
        };

        internal double Evaluate() {
            double result = this.BaseValue;
            foreach (Modifier modifier in this.Modifiers) {
                result = this.AttributeType.Clamp(modifier.Modify(result), this.Source);
            }
            
            return result;
        }

        internal AttributeQuery(IAttributeReader source, GameplayAttributeType id, double value = 0) {
            this.Source = source;
            this.AttributeType = id;
            this.BaseValue = value;
        }
    }
}