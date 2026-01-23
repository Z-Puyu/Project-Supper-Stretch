using System.Threading;
using CommonFrameworks.Async;
using GameCharacterBehaviours.Runtime.Movement;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayAbilitiesSystem.Runtime.Abilities.Executions;
using UnityEngine;

namespace Characters.Abilities {
    internal sealed class SwitchLocomotionGesture : AbilityExecutionStep {
        [field: SerializeField] private Locomotion.Gesture TargetGesture { get; set; }
        
        protected override Awaitable Execute(Ability.Context context, CancellationToken interrupt) {
            if (context.Source.Root.HasComponent(out Locomotion? component)) {
                component.Mode = this.TargetGesture;
            }
            
            return AsyncTask.CompletedTask;
        }
    }
}
