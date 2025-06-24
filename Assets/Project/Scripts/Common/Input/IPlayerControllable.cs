using Project.Scripts.Player;

namespace Project.Scripts.Common.Input;

public interface IPlayerControllable {
    public abstract void BindInput(InputActions actions);
}
