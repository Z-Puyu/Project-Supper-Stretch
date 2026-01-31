using System;

namespace GameplayAbilities.Attributes {
    public readonly record struct AttributeChange(double OldValue, double NewValue, double Delta) {
        public bool IsNegligible => Math.Abs(this.Delta) < 0.001;
        
        internal AttributeChange(double oldValue, double newValue) : this(oldValue, newValue, newValue - oldValue) { }
        
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