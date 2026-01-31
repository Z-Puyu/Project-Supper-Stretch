using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using AnimationUtilities;
using CommonFrameworks.Async;
using CommonFrameworks.Collections;
using CommonFrameworks.Components;
using GameplayAbilities.Attributes;
using GameplayAbilities.Effects;
using GameplayAbilities.Modifiers;
using GameplayKeywords;
using UnityEngine;
using UnityEngine.Events;
using Attribute = GameplayAbilities.Attributes.Attribute;

namespace GameplayAbilities.Abilities {
    [AddComponentMenu("")]
    public sealed class AbilitySystem : Module,
                                        IEffectEmitterFacade,
                                        IEffectReceiverFacade,
                                        IEnumerable<Ability> {
        private ICollection<Ability> AvailableAbilities { get; } = new HashSet<Ability>();

        private TrieDictionary<Keyword, char, ICollection<Ability>> AbilitiesByTag { get; } =
            new TrieDictionary<Keyword, char, ICollection<Ability>>();

        private IDictionary<Ability, Ability.Context> RunningAbilities { get; } =
            new Dictionary<Ability, Ability.Context>();

        private EffectRegistry EffectRegistry { get; } = new EffectRegistry();

        [NotNull] private AnimationController? AnimationController { get; set; }
        [NotNull] private KeywordContainer? KeywordContainer { get; set; }
        [NotNull] private AttributeSet? AttributeSet { get; set; }
        [NotNull] private IModifiable? ModifierConsumer { get; set; } 
        
        [field: SerializeField] private List<Ability> DefaultAbilities { get; set; } = new List<Ability>();

        public event UnityAction<Ability> OnAbilityStarted = delegate { };
        public event UnityAction<Ability> OnAbilityStopped = delegate { };
        public event UnityAction<Ability> OnAbilityGranted = delegate { };
        public event UnityAction<Ability> OnAbilityRevoked = delegate { };

        protected override void Awake() {
            base.Awake();
            this.KeywordContainer = this.Root.GetOrAdd<KeywordContainer>();
            this.ModifierConsumer = this.AttributeSet = this.Root.GetOrAdd<AttributeSet>();
            this.AnimationController = this.Root.GetOrAdd<AnimationController>();
            foreach (Ability ability in this.DefaultAbilities) {
                this.Grant(ability);
            }
        }

        private void Start() {
            ComponentBindings<Animator, AbilitySystem>.Bind(this.AnimationController.Animator, this);
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
            foreach (Keyword keyword in ability.Tags) {
                if (!this.AbilitiesByTag.TryGetValue(keyword, out ICollection<Ability> abilities)) {
                    abilities = new List<Ability>();
                    this.AbilitiesByTag.Add(keyword, abilities);
                }

                abilities.Add(ability);
            }

            this.OnAbilityGranted.Invoke(ability);
            return true;
        }

        public bool Revoke(Ability ability) {
            if (!this.AvailableAbilities.Remove(ability)) {
                return false;
            }

            foreach (Keyword keyword in ability.Tags) {
                this.AbilitiesByTag[keyword].Remove(ability);
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
        /// Attempts to execute the first ability with the given keyword tag.
        /// </summary>
        /// <param name="keyword">The keyword tag to search for.</param>
        /// <param name="userData">Optional user data for the ability.</param>
        public async Awaitable Perform(Keyword keyword, IReadOnlyDictionary<string, double>? userData = null) {
            Ability? ability = this.AbilitiesByTag
                                   .DepthFirstPrefixSearch(keyword.Value)
                                   .FirstOrDefault().Value.FirstOrDefault();
            if (ability) {
                await this.Perform(ability, userData);
            }
        }

        /// <summary>
        /// Stops the given ability from executing.
        /// </summary>
        /// <param name="ability">The ability to stop.</param>
        public void Stop(Ability ability) {
            if (!this.RunningAbilities.Remove(ability, out Ability.Context context)) {
                return;
            }

            if (!context.MainTask.TryInterrupt() && !context.MainTask.TryComplete()) {
#if DEBUG
                Debug.LogError($"{ability} is running but the asynchronous task has ended unexpectedly.");
#endif
                return;
            }

            this.AvailableAbilities.Add(ability);
            this.OnAbilityStopped.Invoke(ability);
        }

        /// <summary>
        /// Stops all abilities that have the given keyword tag.
        /// </summary>
        /// <param name="keyword">The keyword tag to stop abilities with.</param>
        public void Stop(Keyword keyword) {
            IEnumerable<Ability> abilities = this.AbilitiesByTag
                                                 .DepthFirstPrefixSearch(keyword.Value)
                                                 .SelectMany(pair => pair.Value);
            foreach (Ability ability in abilities) {
                this.Stop(ability);
            }
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

        public Awaitable<AnimationPlayResult> PlayAnimation(
            AnimationClip anim, CancellationToken interrupter, UnityAction<AnimationNotifier> onNotify
        ) {
            return !this.AnimationController
                    ? AsyncTask<AnimationPlayResult>.FromResult(AnimationPlayResult.Invalid)
                    : this.AnimationController.Play(anim, onNotify, interrupter);
        }

        public void SetAnimatorInt(int hash, int value) {
            this.AnimationController.SetInteger(hash, value);
        }
        
        public void SetAnimatorFloat(int hash, float value) {
            this.AnimationController.SetFloat(hash, value);
        }
        
        public void SetAnimatorBool(int hash, bool value) {
            this.AnimationController.SetBool(hash, value);
        }
        
        public void SetAnimatorTrigger(int hash) {
            this.AnimationController.SetTrigger(hash);
        }

        public void ResetAnimatorTrigger(int hash) {
            this.AnimationController.ResetTrigger(hash);
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
        
        IEnumerator<Attribute> IEnumerable<Attribute>.GetEnumerator() {
            return this.AttributeSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        bool ITaggable<Keyword>.Tag(Keyword label) {
            return this.KeywordContainer.Tag(label);
        }

        bool ITaggable<Keyword>.Untag(Keyword keyword) {
            return this.KeywordContainer.Untag(keyword);
        }

        bool ITaggable<Keyword>.HasTag(Keyword keyword) {
            return this.KeywordContainer.HasTag(keyword);
        }

        double IAttributeReader.Query(AttributeKey key) {
            return this.AttributeSet.Query(key);
        }

        double IAttributeReader.QueryMax(AttributeKey key) {
            return this.AttributeSet.QueryMax(key);
        }

        double IAttributeReader.QueryMin(AttributeKey key) {
            return this.AttributeSet.QueryMin(key);
        }

        bool IAttributeReader.HasAtLeast(double threshold, AttributeKey key) {
            return this.AttributeSet.HasAtLeast(threshold, key);
        }

        bool IAttributeReader.HasAtMost(double cap, AttributeKey key) {
            return this.AttributeSet.HasAtMost(cap, key);
        }

        void IModifiable.AddModifier(Modifier modifier) {
            this.ModifierConsumer.AddModifier(modifier);
        }
    }
}
