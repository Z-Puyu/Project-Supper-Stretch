namespace SaveAndLoad.Momentos {
    public interface IMomento {
        protected internal string Id { get; set; }
    }
    
    public interface IMomento<in T> : IMomento {
        public void Capture(T entity);
        public void Restore(T entity);
    }
}
