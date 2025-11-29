namespace GameplayAbilitiesSystem.Runtime.Attributes {
    public readonly struct AttributeChange {
        public AttributeKey Attribute { get; }
        public double OldValue { get; }
        public double NewValue { get; }
        public double Delta { get; }
        
        public AttributeChange(AttributeKey attribute, double oldValue, double newValue) {
            this.Attribute = attribute;
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.Delta = newValue - oldValue;
        }
    }
}
