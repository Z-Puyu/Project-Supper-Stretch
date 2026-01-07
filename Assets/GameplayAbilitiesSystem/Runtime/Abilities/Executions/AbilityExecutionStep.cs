using System;
using System.Threading;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    public abstract class AbilityExecutionStep : IAbilityExecutor {
        protected AbilitySystem? OwnerSystem { get; private set; }
        protected Ability? OwnerAbility { get; private set; }
        [field: SerializeField] protected bool WillEndAbilityOnCompletion { get; private set; }

        async Awaitable IAbilityExecutor.Run(AbilitySystem system, Ability ability, CancellationToken interrupt) {
            this.OwnerSystem = system;
            this.OwnerAbility = ability;
            await this.Execute(system, ability, interrupt);
        }

        /// <summary>
        /// Executes the ability step.
        /// </summary>
        /// <param name="system">The ability system that initiated the ability</param>
        /// <param name="ability">The ability that is being executed</param>
        /// <param name="interrupt">The cancellation token for interrupting the ability externally</param>
        /// <returns>An awaitable that completes when the execution is finished</returns>
        protected abstract Awaitable Execute(
            AbilitySystem system, Ability ability, CancellationToken interrupt
        );

        /// <summary>
        /// CLeans up the ability step when it completes.
        /// </summary>
        /// <param name="system">The ability system that initiated the ability</param>
        /// <param name="ability">The ability that is being executed</param>
        protected virtual void OnComplete(AbilitySystem system, Ability ability) { }
        
        void IAbilityExecutor.Complete() {
            if (!this.OwnerSystem || !this.OwnerAbility) {
                Debug.LogWarning("Ability execution completes when it is not running");
                return;
            }
            
            this.OnComplete(this.OwnerSystem, this.OwnerAbility);
            if (this.WillEndAbilityOnCompletion) {
                this.OwnerSystem.Stop(this.OwnerAbility);
            }
            
            this.OwnerSystem = null;
            this.OwnerAbility = null;
        }
    }
}
