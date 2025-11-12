namespace CommonFrameworks.CommonUtilities.Processors {
    public interface IProcessor<T> {
        public T Process(T data);
    }
}
