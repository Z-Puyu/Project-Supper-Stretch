namespace GameplayAbilities.Effects.Triggers {
    public interface IEffectTriggerCondition<in T> {
        public bool Holds(T context);
    }
}
