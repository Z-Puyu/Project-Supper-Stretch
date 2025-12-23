namespace CommonFrameworks.Utilities;

public interface ICommand {
    public void Execute();
    public void Undo();
}