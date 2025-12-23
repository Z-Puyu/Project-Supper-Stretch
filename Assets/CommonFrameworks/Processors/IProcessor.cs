namespace CommonFrameworks.Processors;

public interface IProcessor<T> {
    public T Process(T data);
}