namespace GameplayAbilitiesSystem.Runtime.Attributes {
    public record struct Attribute {
        public IAttributeReader Source { get; }
        public AttributeKey Id { get; }
        public double Value { get; internal set; }
        internal bool HasBeenApproximated { get; set; } = false;

        internal Attribute(IAttributeReader source, AttributeKey id, double value, bool hasBeenApproximated = false) {
            this.Source = source;
            this.Id = id;
            this.Value = value;
            this.HasBeenApproximated = hasBeenApproximated;
        }
    }
}