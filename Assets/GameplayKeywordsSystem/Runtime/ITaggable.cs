namespace GameplayKeywordsSystem.Runtime {
    public interface ITaggable<in T> {
        public bool Tag(T label);
        public bool Untag(T keyword);
        public bool HasTag(T keyword);
    }
}