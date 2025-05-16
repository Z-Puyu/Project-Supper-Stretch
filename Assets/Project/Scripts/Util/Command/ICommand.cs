namespace Project.Scripts.Util.Command;

public interface ICommand {
    public abstract void Execute();
}

public interface ICommand<in T> {
    public abstract void Execute(T args);
}
