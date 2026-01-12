using System;
using System.Collections.Generic;
using System.Threading;
using CommonFrameworks.Extensions;
using GameplayAbilitiesSystem.Runtime.Effects;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities.Executions {
    [Serializable]
    public sealed class ApplyEffectsOnSelf : AbilityExecutionStep {
        [field: SerializeField] private List<Effect> Effects { get; set; } = new List<Effect>();

        protected override Awaitable Execute(
            AbilitySystem system, Ability ability, CancellationToken interrupt,
            IReadOnlyDictionary<string, double>? userData = null
        ) {
            foreach (Effect effect in this.Effects) {
                effect.Apply(system, system, userData, ability, interrupt);
            }

            return AwaitableTask.CompletedTask;
        }
    }
}
