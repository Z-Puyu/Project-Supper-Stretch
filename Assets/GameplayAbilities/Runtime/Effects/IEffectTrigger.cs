namespace GameplayAbilities.Effects {
    /// <summary>
    /// A trigger that can be used to fire effects based on a given context.
    /// </summary>
    /// <typeparam name="T">The type of the context object used to decide whether the effect should trigger</typeparam>
    public interface IEffectTrigger<in T> {
        public bool ShouldTrigger(T context);
        public void TriggerEffect(T context, Effect effect);
    }
}
