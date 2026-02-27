using GameplayAbilities.Attributes;

namespace GameplayAbilities.Modifiers {
    public interface IModifiable {
        public void AddModifier(GameplayAttributeType target, Modifier modifier);
    }
}