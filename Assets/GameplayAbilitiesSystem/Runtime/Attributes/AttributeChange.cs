namespace GameplayAbilitiesSystem.Runtime.Attributes {
    public readonly record struct AttributeChange(double OldValue, double NewValue, double Delta) {
        internal AttributeChange(double oldValue, double newValue) : this(oldValue, newValue, newValue - oldValue) { }
    }
}