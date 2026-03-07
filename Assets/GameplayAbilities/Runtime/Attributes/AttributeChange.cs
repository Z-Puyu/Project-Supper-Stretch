using System;

namespace GameplayAbilities.Attributes {
    public readonly record struct AttributeChange(
        GameplayAttributeType AttributeType,
        AttributeValue OldValue,
        AttributeValue NewValue,
        double Delta
    ) {
        public bool IsNegligible => Math.Abs(this.Delta) < 0.001;
        public bool IsPositive => !this.IsNegligible && this.Delta > 0;
        public bool IsNegative => !this.IsNegligible && this.Delta < 0;

        internal AttributeChange(GameplayAttributeType attributeType, AttributeValue oldValue, AttributeValue newValue)
                : this(attributeType, oldValue, newValue, newValue.Value - oldValue.Value) { }

        public static bool operator >(AttributeChange change, double threshold) {
            return change.Delta > threshold;
        }

        public static bool operator <(AttributeChange change, double threshold) {
            return change.Delta < threshold;
        }

        public static bool operator >=(AttributeChange change, double threshold) {
            return change.Delta >= threshold;
        }

        public static bool operator <=(AttributeChange change, double threshold) {
            return change.Delta <= threshold;
        }

        public static bool operator ==(AttributeChange change, double value) {
            return Math.Abs(change.Delta - value) < 0.00001;
        }

        public static bool operator !=(AttributeChange change, double value) {
            return Math.Abs(change.Delta - value) >= 0.00001;
        }
    }
}