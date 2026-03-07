namespace GameplayAbilities.Abilities.Predicate {
    public interface IAbilityPrerequisite {
        public bool Holds(AbilitySystem system);
    }
}
