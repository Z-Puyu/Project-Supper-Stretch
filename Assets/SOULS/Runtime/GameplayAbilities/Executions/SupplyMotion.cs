// using System.Threading;
// using CommonFrameworks.Async;
// using GameplayAbilities.Abilities;
// using GameplayAbilities.Abilities.Executions;
// using GameplayBehaviours.Movement;
// using UnityEngine;
//
// namespace SOULS.GameplayAbilities.Executions {
//     public sealed class SupplyMotion : AbilityExecutionStep {
//         [field: SerializeField] private Vector3 Velocity { get; set; } = Vector3.zero;
//         
//         protected override Awaitable Execute(Ability.Context context, CancellationToken interrupt) {
//             if (context.Source.Root.HasModule(out Locomotion? locomotion)) {
//                 locomotion.SupplyVelocity(this.Velocity);
//             }
//             
//             return AsyncTask.CompletedTask;
//         }
//     }
// }
