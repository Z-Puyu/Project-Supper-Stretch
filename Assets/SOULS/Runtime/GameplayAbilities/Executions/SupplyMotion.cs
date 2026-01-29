using System.Threading;
using CommonFrameworks.Async;
using GameCharacterBehaviours.Runtime.Movement;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayAbilitiesSystem.Runtime.Abilities.Executions;
using UnityEngine;

namespace SOULS.Runtime.GameplayAbilities.Executions {
    public sealed class SupplyMotion : AbilityExecutionStep {
        [field: SerializeField] private Vector3 Velocity { get; set; } = Vector3.zero;
        
        protected override Awaitable Execute(Ability.Context context, CancellationToken interrupt) {
            if (context.Source.Root.HasComponent(out Locomotion? locomotion)) {
                locomotion.SupplyVelocity(this.Velocity);
            }
            
            return AsyncTask.CompletedTask;
        }
    }
}
