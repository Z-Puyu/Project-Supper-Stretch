namespace SaveAndLoadSystem.Runtime.Momentos {
    public interface ISaveableData<D, in T> where D : class, ISaveableData<D, T> {
        public void Save(T data);
        public void Load(T data);
    }
}
