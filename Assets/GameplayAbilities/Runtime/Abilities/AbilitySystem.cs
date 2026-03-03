using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Common;
using GameplayAbilities.Effects;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet), typeof(EffectReceiver))]
    public sealed class AbilitySystem : MonoBehaviour, IEnumerable<Ability> {
        private ICollection<Ability> AvailableAbilities { get; } = new HashSet<Ability>();

        private IDictionary<Ability, CancellationTokenSource> RunningAbilities { get; } =
            new Dictionary<Ability, CancellationTokenSource>();

        [NotNull] [field: SerializeField] private GameObject? Owner { get; set; }
        [NotNull] [field: SerializeField] private Animator? Animator { get; set; }
        [NotNull] private AttributeSet? AttributeSet { get; set; }
        [NotNull] private ModifierEnvironment? ModifierEnvironment { get; set; }
        [NotNull] private EffectReceiver? EffectReceiver { get; set; }
        
        [field: SerializeField] private List<Ability> DefaultAbilities { get; set; } = new List<Ability>();

        [field: SerializeField]
        private RuntimeAbilityResourceContainer ResourceContainer { get; set; } = new RuntimeAbilityResourceContainer();

        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
            this.ModifierEnvironment = this.GetComponent<ModifierEnvironment>();
            this.EffectReceiver = this.GetComponent<EffectReceiver>();
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
                this.EffectReceiver.AddEffect(this.AttributeSet, cost, userData);
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
        public async void Perform(AbilityExecutionContext context) {
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

                await ability.Execute(this, context.UserData, linked.Token);
            } catch (OperationCanceledException) { } catch (Exception e) {
#if DEBUG
                Debug.LogException(e);
#endif
            } finally {
                this.Conclude(context.Ability);
            }
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

        public bool IsRunningAbility(Ability ability) {
            return this.RunningAbilities.ContainsKey(ability);
        }

        public void SetAnimatorInt(int hash, int value) {
            this.Animator.SetInteger(hash, value);
        }
        
        public void SetAnimatorFloat(int hash, float value) {
            this.Animator.SetFloat(hash, value);
        }
        
        public void SetAnimatorBool(int hash, bool value) {
            this.Animator.SetBool(hash, value);
        }
        
        public void SetAnimatorTrigger(int hash) {
            this.Animator.SetTrigger(hash);
        }

        public void ResetAnimatorTrigger(int hash) {
            this.Animator.ResetTrigger(hash);
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
