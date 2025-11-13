namespace CommonFrameworks.CommonUtilities.CommonInterfaces {
    public interface IEffect<in T> {
        public void Apply(T target);
        public void Stop();
    }

    public interface IEffect<in S, in T> {
        public void Apply(S source, T target);
        public void Stop();
    }
}
