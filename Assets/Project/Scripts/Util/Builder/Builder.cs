namespace Project.Scripts.Util.Builder;

public class Builder<T> where T : new() {
    private T Template { get; init; } = new T();
    
    public T Build() => this.Template;
}
