using System.Threading;
using CommonFrameworks.Async;
using GameCharacterBehaviours.Runtime.Movement;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayAbilitiesSystem.Runtime.Abilities.Executions;
using UnityEngine;

namespace SOULS.Runtime.GameplayAbilities.Executions {
    internal sealed class SwitchLocomotionGesture : AbilityExecutionStep {
        [field: SerializeField] private Locomotion.Gesture TargetGesture { get; set; }
        
        protected override Awaitable Execute(Ability.Context context, CancellationToken interrupt) {
            if (context.Source.Root.HasComponent(out Locomotion? component)) {
                component.SwitchGesture(this.TargetGesture);
            }
            
            return AsyncTask.CompletedTask;
        }
    }
}
