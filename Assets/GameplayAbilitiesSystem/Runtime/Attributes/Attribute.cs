namespace GameplayAbilitiesSystem.Runtime.Attributes {
    public readonly struct Attribute {
        public IAttributeReader Source { get; }
        public AttributeKey Id { get; }
        public double Value { get; }
        internal bool IsValueApproximated { get; }
        
        internal Attribute(IAttributeReader source, AttributeKey id, double value, bool isValueApproximated) {
            this.Source = source;
            this.Id = id;
            this.Value = value;
            this.IsValueApproximated = isValueApproximated;
        }
    }
}