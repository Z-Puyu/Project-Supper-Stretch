namespace Project.Scripts.Util.Variables;

public interface IReadable<out T> {
    public abstract T? Read();
}
