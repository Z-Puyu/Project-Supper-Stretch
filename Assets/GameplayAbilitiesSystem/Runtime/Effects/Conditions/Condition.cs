using System;
using CommonFrameworks.CommonUtilities.CommonInterfaces;
using GameplayAbilitiesSystem.Runtime.Modifiers;

namespace GameplayAbilitiesSystem.Runtime.Effects.Conditions {
    [Serializable]
    public abstract class Condition : IPredicate<EffectSource>, IPredicate<EffectTarget>, IPredicate<ModifierEnvironment> {
        public abstract bool Holds(EffectSource source);
        public abstract bool Holds(EffectTarget target);

        public virtual bool Holds(ModifierEnvironment environment) {
            return true;
        }
    }
}
