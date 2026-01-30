using System;
using CommonFrameworks.Logic;
using GameCharacterBehaviours.Runtime.Movement;
using GameplayAbilitiesSystem.Runtime.Abilities;
using UnityEngine;

namespace SOULS.Runtime.GameplayAbilities.Predicates { 
    [Serializable]
    internal struct MatchLocomotionGesture : IPredicate<AbilitySystem> {
        [field: SerializeField] private Locomotion.Gesture RequiredGesture { get; set; }
        
        public bool Holds(AbilitySystem args) {
            return args.Root.HasModule(out Locomotion? component) &&
                   component.Mode == this.RequiredGesture;
        }
        
        public override string ToString() {
            return $"Is in {this.RequiredGesture} gesture";
        }
    }
}
