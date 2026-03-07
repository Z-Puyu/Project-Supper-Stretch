using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Common;
using GameplayAbilities.Effects;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet), typeof(EffectReceiver))]
    public sealed class AbilitySystem : MonoBehaviour, IEnumerable<Ability> {
        private ICollection<Ability> AvailableAbilities { get; } = new HashSet<Ability>();

        private IDictionary<Ability, CancellationTokenSource> RunningAbilities { get; } =
            new Dictionary<Ability, CancellationTokenSource>();

        [NotNull] private AbilitySystemController? AbilitySystemController { get; set; }
        [NotNull] [field: SerializeField] private GameObject? Owner { get; set; }
        [NotNull] [field: SerializeField] private Animator? Animator { get; set; }
        [NotNull] private AttributeSet? AttributeSet { get; set; }
        [NotNull] private EffectReceiver? EffectReceiver { get; set; }
        [field: SerializeField] private List<Ability> DefaultAbilities { get; set; } = new List<Ability>();

        [field: SerializeField, Inline]
        private RuntimeAbilityResourceContainer ResourceContainer { get; set; } = new RuntimeAbilityResourceContainer();

        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
            this.EffectReceiver = this.GetComponent<EffectReceiver>();
            this.AbilitySystemController = new AbilitySystemController(
                this.Owner, this, this.EffectReceiver, this.ResourceContainer, this.AttributeSet, this.Animator
            );
        }

        private void Start() {
            this.ResourceContainer.RegisterResources();
            foreach (Ability ability in this.DefaultAbilities) {
                this.Grant(ability);
            }
        }

        /// <summary>
        /// Grants the given ability to the ability system.
        /// </summary>
        /// <param name="ability">The ability to grant.</param>
        /// <returns>True if the ability was granted, false if it was already granted.</returns>
        public bool Grant(Ability ability) {
            if (this.AvailableAbilities.Contains(ability)) {
                return false;
            }

            this.AvailableAbilities.Add(ability);
            return true;
        }

        public bool Revoke(Ability ability) {
            return this.AvailableAbilities.Remove(ability);
        }

        internal bool TrySpend(IEnumerable<Cost> costs, IUserData? userData) {
            Cost[] array = costs.ToArray();
            if (Array.Exists(array, cost => !cost.IsAffordable(this.AttributeSet))) {
                return false;
            }
            
            foreach (Cost cost in array) {
                this.EffectReceiver.RegisterEffect(this.AttributeSet, cost, userData);
            }
            
            return true;
        }
        
        /// <summary>
        /// Attempts to execute the given ability. This will check if the ability system has the ability and
        /// the conditions for the ability to start are met.
        /// </summary>
        /// <param name="ability">The ability to perform.</param>
        /// <returns>An awaitable that completes when the ability has finished executing.</returns>
        public void Perform(Ability ability) {
            this.Perform(new AbilityExecutionContext(ability, null));
        }

        /// <summary>
        /// Attempts to execute the given ability. This will check if the ability system has the ability and
        /// the conditions for the ability to start are met.
        /// </summary>
        /// <param name="context">The context of the ability execution.</param>
        /// <returns>An awaitable that completes when the ability has finished executing.</returns>
        public void Perform(AbilityExecutionContext context) {
            this.PerformFireAndForget(context);
        }

        private async void PerformFireAndForget(AbilityExecutionContext context) {
            await this.PerformInternal(context);
        }

        private async Awaitable PerformInternal(AbilityExecutionContext context) {
            try {
                Ability ability = context.Ability;
                this.Stop(ability);
                if (!this.AvailableAbilities.Remove(ability)) {
                    return;
                }

                if (!ability.TryCommit(this, context.UserData)) {
                    this.AvailableAbilities.Add(ability);
                    return;
                }

                CancellationTokenSource cts = new CancellationTokenSource();
                this.RunningAbilities.Add(ability, cts);
                using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(
                    cts.Token, this.destroyCancellationToken
                );

                await ability.Execute(this.AbilitySystemController, context.UserData, linked.Token);
            } catch (OperationCanceledException) { } catch (Exception e) {
                this.LogExecutionFailure(context.Ability, e);
            } finally {
                this.Conclude(context.Ability);
            }
        }

        private void LogExecutionFailure(Ability ability, Exception exception) {
            Debug.LogError($"Ability execution failed. Ability: {ability.name}.", this);
            Debug.LogException(exception, this);
        }

        private void Conclude(Ability ability) {
            if (!this.RunningAbilities.Remove(ability, out CancellationTokenSource cts)) {
                return;
            }
            
            this.AvailableAbilities.Add(ability);
            cts.Dispose();
        }

        /// <summary>
        /// Stops the given ability from executing.
        /// </summary>
        /// <param name="ability">The ability to stop.</param>
        public void Stop(Ability ability) {
            if (!this.RunningAbilities.TryGetValue(ability, out CancellationTokenSource interrupter)) {
                return;
            }

            interrupter.Cancel();
        }

        public IEnumerator<Ability> GetEnumerator() {
            foreach (Ability ability in this.AvailableAbilities) {
                yield return ability;
            }

            foreach (Ability ability in this.RunningAbilities.Keys) {
                yield return ability;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
