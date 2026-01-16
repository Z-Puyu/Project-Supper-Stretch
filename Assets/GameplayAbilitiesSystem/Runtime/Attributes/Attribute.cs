namespace GameplayAbilitiesSystem.Runtime.Attributes {
    public record struct Attribute {
        public IAttributeReader Source { get; }
        public AttributeKey Id { get; }
        public double Value { get; internal set; }

        internal Attribute(IAttributeReader source, AttributeKey id, double value) {
            this.Source = source;
            this.Id = id;
            this.Value = value;
        }
    }
}