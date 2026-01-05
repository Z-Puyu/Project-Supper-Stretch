using System;
using System.Threading;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    public abstract class AbilityExecutionStep {
        protected AbilitySystem? OwnerSystem { get; private set; }
        protected Ability? OwnerAbility { get; private set; }
        internal bool IsComplete { get; private set; }
        [field: SerializeField] protected bool WillEndAbilityOnCompletion { get; private set; }

        public async Awaitable Run(AbilitySystem system, Ability ability, CancellationTokenSource interrupter) {
            this.OwnerSystem = system;
            this.OwnerAbility = ability;
            await this.Execute(system, ability, interrupter);
        }

        protected abstract Awaitable Execute(
            AbilitySystem system, Ability ability, CancellationTokenSource interrupter
        );

        protected virtual void OnComplete(AbilitySystem system, Ability ability) { }
        
        internal void Complete() {
            if (!this.OwnerSystem || !this.OwnerAbility) {
                Debug.LogWarning("Ability execution completes when it is not running");
                return;
            }
            
            this.OnComplete(this.OwnerSystem, this.OwnerAbility);
            this.IsComplete = true;
            if (this.WillEndAbilityOnCompletion) {
                this.OwnerSystem.Stop(this.OwnerAbility);
            }
            
            this.OwnerSystem = null;
            this.OwnerAbility = null;
        }
    }
}
