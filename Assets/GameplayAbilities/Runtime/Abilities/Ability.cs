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
        private static readonly HashSet<Ability> Instances = new HashSet<Ability>();
        
        [field: SerializeField] private List<Cost> Costs { get; set; } = new List<Cost>();
        [field: SerializeField, Min(0)] internal float Cooldown { get; private set; }

        [field: SerializeReference, Tooltip("Conditions on the ability system for this ability to be usable")]
        private List<IAbilityPrerequisite> Conditions { get; set; } = new List<IAbilityPrerequisite>();
        
        [field: SerializeReference, SubtypeSelector] private AbilityExecution? Execution { get; set; }

        private void OnEnable() {
            Ability.Instances.Add(this);
        }
        
        private void OnDisable() {
            Ability.Instances.Remove(this);
        }

        internal static IEnumerable<AbilityResourceKey<T>> ExtractAllResourceKeys<T>() {
            return Ability.Instances.SelectMany(a => a.ExtractResourceKeys<T>());
        }

        private IEnumerable<AbilityResourceKey<T>> ExtractResourceKeys<T>() {
            return this.Execution?.GetType()
                       .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                       .Select(field => field.GetValue(this.Execution))
                       .OfType<AbilityResourceKey<T>>()
                       .Where(key => !key.IsEmpty) ?? Enumerable.Empty<AbilityResourceKey<T>>();
        }

        internal bool RequiresResource<T>(AbilityResourceKey<T> key) {
            return this.Execution?.GetType()
                       .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                       .Select(field => field.GetValue(this.Execution))
                       .OfType<AbilityResourceKey<T>>()
                       .Any(key.IsSameKey) ?? false;
        }

        internal bool TryCommit(AbilitySystem system, IUserData? userData) {
            foreach (IAbilityPrerequisite condition in this.Conditions) {
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

        private void OnValidate() {
            int a = 0;
        }
    }
}
