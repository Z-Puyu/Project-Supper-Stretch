namespace GameplayAbilities.Effects {
    public interface IEffect<T> {
        public void Apply(T target);
        public void Stop();
        public void Complete();
    }
}
