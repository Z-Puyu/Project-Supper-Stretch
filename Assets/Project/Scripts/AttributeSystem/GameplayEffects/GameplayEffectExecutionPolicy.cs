namespace Project.Scripts.AttributeSystem.GameplayEffects;

public enum GameplayEffectExecutionPolicy {
    AbortIfAnyInvalid,
    IgnoreInvalid,
    AlwaysExecuteAll,
    QueueInvalid
}
