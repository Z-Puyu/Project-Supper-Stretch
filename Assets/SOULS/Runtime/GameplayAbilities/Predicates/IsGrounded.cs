// using System;
// using CommonFrameworks.Logic;
// using GameplayAbilities.Abilities;
// using GameplayBehaviours.Movement;
//
// namespace SOULS.GameplayAbilities.Predicates {
//     [Serializable]
//     internal struct IsGrounded : IPredicate<AbilitySystem> {
//         public bool Holds(AbilitySystem args) {
//             return !args.Root.HasModule(out Locomotion? locomotion) || locomotion.IsGrounded;
//         }
//         
//         public override string ToString() {
//             return "Is grounded";
//         }
//     }
// }
