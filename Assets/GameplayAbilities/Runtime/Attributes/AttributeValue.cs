namespace GameplayAbilities.Attributes {
    public readonly record struct AttributeValue(
        double BaseValue,
        double Value,
        double PreciseValue
    ) {
        internal static readonly AttributeValue Zero = new AttributeValue(0, 0, 0);
        
        public double BonusValue => this.Value - this.BaseValue;
    }
}