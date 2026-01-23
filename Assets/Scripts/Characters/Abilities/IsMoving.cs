using CommonFrameworks.Logic;
using GameCharacterBehaviours.Runtime.Movement;
using GameplayAbilitiesSystem.Runtime.Abilities;

namespace Characters.Abilities {
    internal sealed class IsMoving : IPredicate<AbilitySystem> {
        public bool Holds(AbilitySystem args) {
            return args.Root.HasComponent(out Locomotion? component) && component.IsMoving;
        }
        
        public override string ToString() {
            return "Is moving";
        }
    }
}
