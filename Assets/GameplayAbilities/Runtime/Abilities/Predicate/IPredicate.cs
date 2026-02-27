namespace GameplayAbilities.Abilities.Predicate {
    public interface IPredicate<in T> {
        public bool Holds(T args);
    }
}
