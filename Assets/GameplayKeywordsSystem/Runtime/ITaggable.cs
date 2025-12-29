namespace GameplayKeywordsSystem.Runtime {
    public interface ITaggable<in T> {
        public bool Add(T label);
        public bool Remove(T label);
        public bool Contains(T label);
        public void Clear();
    }
}