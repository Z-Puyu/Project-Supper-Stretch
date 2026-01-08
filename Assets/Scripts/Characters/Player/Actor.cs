using System;
using System.Diagnostics.CodeAnalysis;
using Characters.Events;
using CommonFrameworks.Components;
using CommonFrameworks.Events;
using GameCharacterBehaviours.Runtime.Movement;
using GameplayAbilitiesSystem.Runtime.Abilities;
using SaintsField;
using UnityEngine;

namespace Characters.Player {
    [DisallowMultipleComponent]
    public sealed class Actor : MonoBehaviour {
        [NotNull] 
        [field: SerializeField, Required] 
        private AbilitySystem? AbilitySystem { get; set; }
        
        [NotNull]
        [field: SerializeField, Required] 
        private ComponentManager? ComponentRoot { get; set; }
        
        [field: SerializeField] private Ability? RollAbility { get; set; }
        [field: SerializeField] private Ability? BackstepAbility { get; set; }
        [field: SerializeField] private Ability? SprintAbility { get; set; }

        private void Awake() {
            if (!this.ComponentRoot) {
                this.ComponentRoot = this.GetComponentInChildren<ComponentManager>(true);
            }
        }

        private void OnEnable() {
            this.Subscribe<PlayerInputInterpreter, AttemptToDodgeMessage>(this.HandleDodgeEvent);
            this.Subscribe<PlayerInputInterpreter, PerformSprintingMessage>(this.HandleSprintEvent);
        }

        private void OnDisable() {
            this.Mute();
        }

        private T GetActorComponent<T>() where T : BehaviourComponent {
            return this.ComponentRoot.GetOrAdd<T>();
        }
        
        private void HandleDodgeEvent() {
            this.GetActorComponent<AbilitySystem>().Perform(
                this.GetActorComponent<Locomotion>().IsMoving ? this.RollAbility : this.BackstepAbility
            );
        }
        
        private void HandleSprintEvent(Event<PlayerInputInterpreter, PerformSprintingMessage> @event) {
            if (@event.Message.IsSprinting) {
                this.GetActorComponent<AbilitySystem>().Perform(this.SprintAbility);
            } else {
                this.GetActorComponent<AbilitySystem>().Stop(this.SprintAbility);
            }
        }
    }
}
