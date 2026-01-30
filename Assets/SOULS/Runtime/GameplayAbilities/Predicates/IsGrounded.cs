using System;
using CommonFrameworks.Logic;
using GameCharacterBehaviours.Runtime.Movement;
using GameplayAbilitiesSystem.Runtime.Abilities;

namespace SOULS.Runtime.GameplayAbilities.Predicates {
    [Serializable]
    internal struct IsGrounded : IPredicate<AbilitySystem> {
        public bool Holds(AbilitySystem args) {
            return !args.Root.HasModule(out Locomotion? locomotion) || locomotion.IsGrounded;
        }
        
        public override string ToString() {
            return "Is grounded";
        }
    }
}
