namespace GameplayKeywordsSystem.Runtime {
    public interface ITaggable<in T> {
        public bool Add(T label);
        public bool Remove(T keyword);
        public bool Contains(T keyword);
        public void Clear();
    }
}