namespace GameplayAbilities.Runtime.EditorTooling {
    public sealed class SubtypeSelectorAttribute : CustomPropertyAttribute {
        public string PredicateName { get; set; }

        public SubtypeSelectorAttribute(string predicate = "") {
            this.PredicateName = predicate;
        }
    }
}
