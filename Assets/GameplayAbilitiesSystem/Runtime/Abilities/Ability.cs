using System;
using System.Collections.Generic;
using System.Threading;
using CommonFrameworks.Async;
using CommonFrameworks.Extensions;
using CommonFrameworks.Logic;
using GameplayAbilitiesSystem.Runtime.Abilities.Executions;
using GameplayAbilitiesSystem.Runtime.Effects;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability")]
    public sealed class Ability : ScriptableObject {
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))]
        internal List<string> Tags { get; private set; } = new List<string>();

        [field: SerializeField] private List<Cost> Costs { get; set; } = new List<Cost>();

        [field: SerializeReference, Tooltip("Conditions on the ability system for this ability to be usable")]
        [field: FieldLabelText(nameof(this.LabelCondition), true)]
        private List<IPredicate<AbilitySystem>> Conditions { get; set; } = new List<IPredicate<AbilitySystem>>();

        [field: SerializeReference, ReferencePicker]
        private List<IAbilityExecutor> ExecutionSteps { get; set; } = new List<IAbilityExecutor>();

        [field: SerializeField] private AbilityEffect? SideEffect { get; set; }

        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.FetchLeaves<AbilityTagSheet>();

        private string LabelCondition(object condition) {
            return condition.GetType().Name;
        }

        internal bool TryCommit(
            AbilitySystem system, IReadOnlyDictionary<string, double>? userData, out Context context
        ) {
            foreach (IPredicate<AbilitySystem> condition in this.Conditions) {
                if (condition.Holds(system)) {
                    continue;
                }

                context = default;
                return false;
            }

            foreach (Cost cost in this.Costs) {
                if (cost.IsAffordable(system)) {
                    continue;
                }

                context = default;
                return false;
            }

            foreach (Cost cost in this.Costs) {
                cost.Spend(system, system);
            }

            context = new Context(system, this, userData);
            return true;
        }

        internal async Awaitable Execute(Context context) {
            this.SideEffect?.Apply(context.Source, context.UserData);
            using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(
                context.Source.destroyCancellationToken
            );
            
            _ = this.ExecuteSteps(context, linked.Token);
            try {
                await context.MainProcess;
            } catch (OperationCanceledException) {
                linked.Cancel();
            } finally {
                this.SideEffect?.Stop(context.Source);
            }
        }

        private async Awaitable ExecuteSteps(Context context, CancellationToken interrupt) {
            for (int i = 0; i < this.ExecutionSteps.Count; i += 1) {
                if (!await this.ExecutionSteps[i].Run(context, interrupt)) {
                    break;
                }
            }
        }

        public readonly record struct Context(
            AbilitySystem Source,
            Ability Ability,
            IReadOnlyDictionary<string, double>? UserData
        ) {
            public AsyncTask MainTask { get; } = new AsyncTask();
            internal Awaitable MainProcess => this.MainTask.Awaitable;
        }
    }
}
