using System;
using CommonFrameworks.Logic;
using GameplayAbilities.Abilities;
using GameplayBehaviours.Movement;
using UnityEngine;

namespace SOULS.GameplayAbilities.Predicates { 
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
