namespace GameplayAbilities.Runtime.EditorTooling {
    public sealed class ValidateAttribute : CustomPropertyAttribute {
        public string PredicateName { get; }
        
        public ValidateAttribute(string predicate) {
            this.PredicateName = predicate;
        }
    }
}
