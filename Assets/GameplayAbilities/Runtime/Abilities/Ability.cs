using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using GameplayAbilities.Abilities.Predicate;
using GameplayAbilities.Common;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [CreateAssetMenu(fileName = "New Ability", menuName = "Gameplay Abilities/Ability")]
    public sealed class Ability : ScriptableObject {
        [field: SerializeField] private List<Cost> Costs { get; set; } = new List<Cost>();

        [field: SerializeReference, Tooltip("Conditions on the ability system for this ability to be usable")]
        private List<IPredicate<AbilitySystem>> Conditions { get; set; } = new List<IPredicate<AbilitySystem>>();
        
        [field: SerializeReference, SubtypeSelector] private AbilityExecution? Execution { get; set; }
        [field: SerializeField] private AbilityResourceKey<string> TestResource { get; set; }

        internal IEnumerable<AbilityResourceKey<T>> ExtractResourceKeys<T>() {
            return this.Execution?.GetType()
                       .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                       .Select(field => field.GetValue(this.Execution))
                       .OfType<AbilityResourceKey<T>>()
                       .Where(key => !key.IsEmpty) ??
                   Enumerable.Empty<AbilityResourceKey<T>>();
        }

        internal bool TryCommit(AbilitySystem system, IUserData? userData) {
            foreach (IPredicate<AbilitySystem> condition in this.Conditions) {
                if (condition.Holds(system)) {
                    continue;
                }
                
                return false;
            }

            return system.TrySpend(this.Costs, userData);
        }

        internal Awaitable Execute(AbilitySystemController controller, IUserData? userData, CancellationToken interrupt) {
            if (this.Execution is not null) {
                return this.Execution.Execute(controller, userData, interrupt);
            }

            AwaitableCompletionSource completed = new AwaitableCompletionSource();
            completed.Reset();
            completed.SetResult();
            return completed.Awaitable;
        }
    }
}
