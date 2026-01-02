using System;
using System.Diagnostics.CodeAnalysis;
using Characters.Events;
using CommonFrameworks.Events;
using GameplayAbilitiesSystem.Runtime.Abilities;
using SaintsField;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Player {
    [DisallowMultipleComponent]
    public sealed class Actor : MonoBehaviour {
        [NotNull] 
        [field: SerializeField, Required] 
        private AbilitySystem? AbilitySystem { get; set; }
        
        [field: SerializeField] private Ability? RollAbility { get; set; }

        private void OnEnable() {
            this.Subscribe<PlayerInputInterpreter, AttemptToDodgeMessage>(this.HandleDodgeEvent);
        }


        private void HandleDodgeEvent() {
            if (!this.RollAbility) {
                return;
            }
            
            this.AbilitySystem.PerformAbility(this.RollAbility);
        }
    }
}
