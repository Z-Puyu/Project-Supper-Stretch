using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using GameplayAbilities.Abilities.Predicate;
using GameplayAbilities.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability")]
    public sealed class Ability : ScriptableObject {
        [field: SerializeField] internal List<string> Tags { get; private set; } = new List<string>();

        [field: SerializeField] private List<Cost> Costs { get; set; } = new List<Cost>();

        [field: SerializeReference, Tooltip("Conditions on the ability system for this ability to be usable")]
        private List<IPredicate<AbilitySystem>> Conditions { get; set; } = new List<IPredicate<AbilitySystem>>();
        
        // [field: SerializeField] private List<GameEventHandler> EventHandlers { get; set; } = new List<GameEventHandler>();
        [field: SerializeReference, SubtypeSelector] private AbilityExecution? Execution { get; set; }
        
        // [field: SerializeField] private AbilityEffect? SideEffect { get; set; }

        private string LabelCondition(object condition) {
            return $"{condition}";
        }

        internal IEnumerable<AbilityResourceKey<T>> ExtractResourceKeys<T>() {
            return this.Execution?.GetType()
                       .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                       .Select(field => field.GetValue(this.Execution))
                       .OfType<AbilityResourceKey<T>>()
                       .Where(key => !key.IsEmpty) ??
                   Enumerable.Empty<AbilityResourceKey<T>>();
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
            //this.SideEffect?.Apply(context.Source, context.UserData);
            using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(
                context.Source.destroyCancellationToken
            );
            
            _ = this.ExecuteSteps(context, linked.Token);
            try {
                //await context.MainProcess;
            } catch (OperationCanceledException) {
                linked.Cancel();
            } finally {
                //this.SideEffect?.Stop(context.Source);
            }
        }

        private async Awaitable ExecuteSteps(Context context, CancellationToken interrupt) {
            // for (int i = 0; i < this.ExecutionSteps.Count; i += 1) {
            //     if (!await this.ExecutionSteps[i].Run(context, interrupt)) {
            //         break;
            //     }
            // }
        }

        public readonly record struct Context(
            AbilitySystem Source,
            Ability Ability,
            IReadOnlyDictionary<string, double>? UserData
        ) {
            //public AsyncTask MainTask { get; } = new AsyncTask();
            //internal Awaitable MainProcess => this.MainTask.Awaitable;
        }
    }
}
