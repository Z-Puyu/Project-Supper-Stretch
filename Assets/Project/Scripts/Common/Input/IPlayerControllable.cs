namespace Project.Scripts.Common.Input;

public interface IPlayerControllable {
    public abstract void BindInput(InputActions actions);
    public abstract void UnbindInput(InputActions actions);
}
