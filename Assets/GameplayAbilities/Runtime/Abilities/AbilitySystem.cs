using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Effects;
using GameplayAbilities.Modifiers;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilities.Abilities {
    [AddComponentMenu("")]
    public sealed class AbilitySystem : MonoBehaviour,
                                        IEffectEmitterFacade,
                                        IEffectReceiverFacade,
                                        IEnumerable<Ability> {
        private ICollection<Ability> AvailableAbilities { get; } = new HashSet<Ability>();

        private IDictionary<Ability, Ability.Context> RunningAbilities { get; } =
            new Dictionary<Ability, Ability.Context>();

        private EffectRegistry EffectRegistry { get; } = new EffectRegistry();

        [NotNull] [field: SerializeField] private Animator? Animator { get; set; }
        [NotNull] [field: SerializeField] private AttributeSet? AttributeSet { get; set; }
        [NotNull] private IModifiable? ModifierConsumer { get; set; } 
        
        [field: SerializeField] private List<Ability> DefaultAbilities { get; set; } = new List<Ability>();

        [field: SerializeField]
        private RuntimeAbilityResourceContainer ResourceContainer { get; set; } = new RuntimeAbilityResourceContainer();

        public event UnityAction<Ability> OnAbilityStarted = delegate { };
        public event UnityAction<Ability> OnAbilityStopped = delegate { };
        public event UnityAction<Ability> OnAbilityGranted = delegate { };
        public event UnityAction<Ability> OnAbilityRevoked = delegate { };

        private void Awake() {
            this.ModifierConsumer = this.AttributeSet;
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
            this.OnAbilityGranted.Invoke(ability);
            return true;
        }

        public bool Revoke(Ability ability) {
            if (!this.AvailableAbilities.Remove(ability)) {
                return false;
            }
            
            this.OnAbilityRevoked.Invoke(ability);
            return true;
        }
        
        /// <summary>
        /// Attempts to execute the given ability. This will check if the ability system has the ability and
        /// the conditions for the ability to start are met.
        /// </summary>
        /// <param name="ability">The ability to perform.</param>
        /// <returns>An awaitable that completes when the ability has finished executing.</returns>
        public async void Perform(Ability ability) {
            try {
                await this.Perform(ability, null);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Attempts to execute the given ability. This will check if the ability system has the ability and
        /// the conditions for the ability to start are met.
        /// </summary>
        /// <param name="ability">The ability to perform.</param>
        /// <param name="userData">Optional user data for the ability.</param>
        /// <returns>An awaitable that completes when the ability has finished executing.</returns>
        public async Awaitable Perform(Ability ability, IReadOnlyDictionary<string, double>? userData) {
            this.Stop(ability);
            if (!this.AvailableAbilities.Remove(ability)) {
                return;
            }
            
            if (!ability.TryCommit(this, userData, out Ability.Context context)) {
                this.AvailableAbilities.Add(ability);
                return;
            }

            this.RunningAbilities[ability] = context;
            Awaitable execution = ability.Execute(context);
            this.OnAbilityStarted.Invoke(ability);
            await execution;
        }

        /// <summary>
        /// Stops the given ability from executing.
        /// </summary>
        /// <param name="ability">The ability to stop.</param>
        public void Stop(Ability ability) {
            if (!this.RunningAbilities.Remove(ability, out Ability.Context context)) {
                return;
            }

//             if (!context.MainTask.TryInterrupt() && !context.MainTask.TryComplete()) {
// #if DEBUG
//                 Debug.LogError($"{ability} is running but the asynchronous task has ended unexpectedly.");
// #endif
//                 return;
//             }

            this.AvailableAbilities.Add(ability);
            this.OnAbilityStopped.Invoke(ability);
        }

        public bool IsRunningAbility(Ability ability) {
            return this.RunningAbilities.ContainsKey(ability);
        }

        /// <summary>
        /// Stops the ability and removes all effects associated with it.
        /// </summary>
        /// <param name="ability">The ability to stop and clean up.</param>
        public void CompletelyStop(Ability ability) {
            this.Stop(ability);
            this.EffectRegistry.Stop(new EffectDescriptor(ability));
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

        CancellationToken IEffectReceiverFacade.Register(EffectDescriptor effect) {
            return this.EffectRegistry.Register(effect);
        }

        void IEffectReceiverFacade.StopEffects(EffectDescriptor effect) {
            this.EffectRegistry.Stop(effect);
        }

        void IEffectEmitterFacade.Apply(Effect effect, IEffectReceiverFacade target) {
            effect.Apply(this, target);
        }

        public IEnumerator<Ability> GetEnumerator() {
            foreach (Ability ability in this.AvailableAbilities) {
                yield return ability;
            }

            foreach (Ability ability in this.RunningAbilities.Keys) {
                yield return ability;
            }
        }
        
        IEnumerator<GameplayAttribute> IEnumerable<GameplayAttribute>.GetEnumerator() {
            return this.AttributeSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        AttributeValue IAttributeReader.Query(GameplayAttributeType key) {
            return this.AttributeSet.Query(key);
        }

        double IAttributeReader.QueryMax(GameplayAttributeType key) {
            return this.AttributeSet.QueryMax(key);
        }

        double IAttributeReader.QueryMin(GameplayAttributeType key) {
            return this.AttributeSet.QueryMin(key);
        }

        bool IAttributeReader.HasAtLeast(double threshold, GameplayAttributeType key) {
            return this.AttributeSet.HasAtLeast(threshold, key);
        }

        bool IAttributeReader.HasAtMost(double cap, GameplayAttributeType key) {
            return this.AttributeSet.HasAtMost(cap, key);
        }

        void IModifiable.AddModifier(GameplayAttributeType target, Modifier modifier) {
            this.ModifierConsumer.AddModifier(target, modifier);
        }
    }
}
