namespace GameplayAbilities.Attributes {
    /// <summary>
    /// The value of an attribute.
    /// </summary>
    /// <param name="BaseValue">The base value of the attribute.</param>
    /// <param name="Value">The current effective value of the attribute.
    /// Usually, this is the value to be used for arithmetic and comparisons.</param>
    /// <param name="PreciseValue">The precise value of the attribute
    /// Usually, this value is used for UI display changes only.</param>
    public readonly record struct AttributeValue(
        double BaseValue,
        double Value,
        double PreciseValue
    ) {
        internal static readonly AttributeValue Zero = new AttributeValue(0, 0, 0);
        
        /// <summary>
        /// The component of the attribute value contributed by modifiers.
        /// </summary>
        public double BonusValue => this.Value - this.BaseValue;
    }
}