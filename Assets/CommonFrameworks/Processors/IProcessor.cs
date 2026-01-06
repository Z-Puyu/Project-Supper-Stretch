namespace CommonFrameworks.Processors {
    public interface IProcessor<T> {
        public void Process(ref T data);
    }
}