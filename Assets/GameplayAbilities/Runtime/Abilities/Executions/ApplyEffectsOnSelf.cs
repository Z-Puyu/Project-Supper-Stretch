using System;
using System.Collections.Generic;
using System.Threading;
using CommonFrameworks.Async;
using GameplayAbilities.Effects;
using UnityEngine;

namespace GameplayAbilities.Abilities.Executions {
    [Serializable]
    public sealed class ApplyEffectsOnSelf : AbilityExecutionStep {
        [field: SerializeField] private List<Effect> Effects { get; set; } = new List<Effect>();

        protected override Awaitable Execute(Ability.Context context, CancellationToken interrupt) {
            foreach (Effect effect in this.Effects) {
                effect.Apply(context.Source, context.Source, context.UserData, context.Ability);
            }

            return AsyncTask.CompletedTask;
        }
    }
}
