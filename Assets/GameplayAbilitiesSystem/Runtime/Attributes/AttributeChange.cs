using System;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    public readonly record struct AttributeChange(double OldValue, double NewValue, double Delta) {
        public bool IsNegligible => Math.Abs(this.Delta) < 0.001;
        
        internal AttributeChange(double oldValue, double newValue) : this(oldValue, newValue, newValue - oldValue) { }
    }
}