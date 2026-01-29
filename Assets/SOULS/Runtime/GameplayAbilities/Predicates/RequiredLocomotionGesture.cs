using System;
using CommonFrameworks.Logic;
using GameCharacterBehaviours.Runtime.Movement;
using GameplayAbilitiesSystem.Runtime.Abilities;
using UnityEngine;

namespace SOULS.Runtime.GameplayAbilities.Predicates { 
    [Serializable]
    internal sealed class RequiredLocomotionGesture : IPredicate<AbilitySystem> {
        [field: SerializeField] private Locomotion.Gesture RequiredGesture { get; set; }
        
        public bool Holds(AbilitySystem args) {
            return args.Root.HasComponent(out Locomotion? component) &&
                   component.CurrentGesture == this.RequiredGesture;
        }
        
        public override string ToString() {
            return $"Is in {this.RequiredGesture} gesture";
        }
    }
}
