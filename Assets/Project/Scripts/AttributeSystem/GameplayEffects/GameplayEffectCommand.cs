using Project.Scripts.Util.Command;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

public abstract class GameplayEffectCommand : ICommand {
    public abstract bool CanExecute();

    public abstract void Execute();
}
