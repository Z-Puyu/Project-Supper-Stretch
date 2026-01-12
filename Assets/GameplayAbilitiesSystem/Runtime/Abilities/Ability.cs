using System;
using System.Collections.Generic;
using System.Threading;
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

        internal bool TryCommit(AbilitySystem system, out AbilityActivation activation) {
            foreach (IPredicate<AbilitySystem> condition in this.Conditions) {
                if (condition.Holds(system)) {
                    continue;
                }

                activation = default;
                return false;
            }
            
            foreach (Cost cost in this.Costs) {
                if (cost.IsAffordable(system.AttributeReader)) {
                    continue;
                }

                activation = default;
                return false;
            }

            foreach (Cost cost in this.Costs) {
                cost.Spend(system.AttributeReader, system.ModifierConsumer);
            }

            activation = new AbilityActivation(new CancellationTokenSource());
            return true;
        }

        internal async Awaitable Execute(
            AbilitySystem system, IReadOnlyDictionary<string, double>? userData, CancellationToken interrupt
        ) {
            this.SideEffect?.Apply(system, userData);
            CancellationToken death = system.destroyCancellationToken;
            using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(interrupt, death);
            for (int i = 0; i < this.ExecutionSteps.Count; i += 1) {
                try {
                    await this.ExecutionSteps[i].Run(system, this, cts.Token, userData);
                    this.ExecutionSteps[i].Complete();
                } catch (OperationCanceledException) {
                    system.Stop(this);
                    break;
                }
            }

            await AwaitableTask.WaitUntilAsync(
                (system, this),
                ((AbilitySystem system, Ability ability) args) => !args.system.IsRunningAbility(args.ability)
            );
            
            this.SideEffect?.Stop(system);
        }
    }
}
