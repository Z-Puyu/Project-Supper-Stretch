namespace GameplayAbilities.Effects {
    public ref struct EffectStackingResult {
        internal bool OverridesLastEffectInstance { get; init; }
        internal EffectExecutionScheme NewEffectExecutionScheme { get; init; }
    }
}
