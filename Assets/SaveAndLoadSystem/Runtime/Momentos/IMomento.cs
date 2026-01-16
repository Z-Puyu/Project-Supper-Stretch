namespace SaveAndLoadSystem.Runtime.Momentos {
    public interface IMomento {
        protected internal string Id { get; set; }
    }
    
    public interface IMomento<in T> : IMomento {
        public void Capture(T transform);
        public void Restore(T transform);
    }
}
