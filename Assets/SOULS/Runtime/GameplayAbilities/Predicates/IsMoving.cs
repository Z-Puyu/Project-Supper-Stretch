using System;
using CommonFrameworks.Logic;
using GameCharacterBehaviours.Runtime.Movement;
using GameplayAbilitiesSystem.Runtime.Abilities;

namespace SOULS.Runtime.GameplayAbilities.Predicates {
    [Serializable]
    internal sealed class IsMoving : IPredicate<AbilitySystem> {
        public bool Holds(AbilitySystem args) {
            return args.Root.HasModule(out Locomotion? component) && component.IsMoving;
        }
        
        public override string ToString() {
            return "Is moving";
        }
    }
}
