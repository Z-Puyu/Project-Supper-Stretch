// using System;
// using CommonFrameworks.Logic;
// using GameplayAbilities.Abilities;
// using GameplayBehaviours.Movement;
//
// namespace SOULS.GameplayAbilities.Predicates {
//     [Serializable]
//     internal struct IsMoving : IPredicate<AbilitySystem> {
//         public bool Holds(AbilitySystem args) {
//             return args.Root.HasModule(out Locomotion? component) && component.IsMoving;
//         }
//         
//         public override string ToString() {
//             return "Is moving";
//         }
//     }
// }
