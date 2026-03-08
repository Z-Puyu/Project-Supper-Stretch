namespace GameplayAbilities.Effects {
    internal readonly record struct StackingResult(bool OverridesLastExecution, int NewStackSize);
}
