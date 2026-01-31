using System.Threading;
using CommonFrameworks.Async;
using GameplayAbilities.Abilities;
using GameplayAbilities.Abilities.Executions;
using GameplayBehaviours.Movement;
using UnityEngine;

namespace SOULS.GameplayAbilities.Executions {
    internal sealed class SwitchLocomotionGesture : AbilityExecutionStep {
        [field: SerializeField] private Locomotion.Gesture TargetGesture { get; set; }
        
        protected override Awaitable Execute(Ability.Context context, CancellationToken interrupt) {
            if (context.Source.Root.HasModule(out Locomotion? component)) {
                component.SwitchGesture(this.TargetGesture);
            }
            
            return AsyncTask.CompletedTask;
        }
    }
}
