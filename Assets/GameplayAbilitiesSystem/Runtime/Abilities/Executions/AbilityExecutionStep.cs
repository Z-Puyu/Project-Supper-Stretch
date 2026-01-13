using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    public abstract class AbilityExecutionStep : IAbilityExecutor {
        [field: SerializeField] protected bool WillEndAbilityOnCompletion { get; private set; }

        async Awaitable<bool> IAbilityExecutor.Run(Ability.Context context, CancellationToken interrupt) {
            try {
                interrupt.ThrowIfCancellationRequested();
                await this.Execute(context, interrupt);
                interrupt.ThrowIfCancellationRequested();
            } catch (OperationCanceledException) {
                return this.OnInterrupt(context.Source, context.Ability);
            }
            
            this.OnComplete(context.Source, context.Ability);
            if (!this.WillEndAbilityOnCompletion) {
                return true;
            }

            context.Source.Stop(context.Ability);
            return false;
        }

        /// <summary>
        /// Executes the ability step.
        /// </summary>
        /// <param name="context">The context of the ability execution</param>
        /// <param name="interrupt">The token provided by the ability owning this execution step to cancel it</param>
        /// <returns>An awaitable that completes when the execution is finished</returns>
        protected abstract Awaitable Execute(Ability.Context context, CancellationToken interrupt);

        /// <summary>
        /// Cleans up the ability step when it completes.
        /// </summary>
        /// <param name="system">The ability system that initiated the ability</param>
        /// <param name="ability">The ability that is being executed</param>
        protected virtual void OnComplete(AbilitySystem system, Ability ability) { }

        /// <summary>
        /// Cleans up the ability step when it is interrupted.
        /// </summary>
        /// <param name="system">The ability system that initiated the ability</param>
        /// <param name="ability">The ability that is being executed</param>
        /// <returns>True if the ability should continue, false otherwise</returns>
        protected virtual bool OnInterrupt(AbilitySystem system, Ability ability) {
            return false;
        }
    }
}
