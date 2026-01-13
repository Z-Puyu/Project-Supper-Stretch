using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using CommonFrameworks.Async;
using CommonFrameworks.Collections;
using CommonFrameworks.Components;
using CommonFrameworks.Extensions;
using GameplayAbilitiesSystem.Runtime.Animations;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Effects;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using Attribute = GameplayAbilitiesSystem.Runtime.Attributes.Attribute;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    [DisallowMultipleComponent]
    public sealed class AbilitySystem : BehaviourComponent,
                                        IEffectEmitterFacade,
                                        IEffectReceiverFacade,
                                        IEnumerable<Ability> {
        private AnimationController? AnimationController { get; set; }
        private ICollection<Ability> AvailableAbilities { get; } = new HashSet<Ability>();

        private TrieDictionary<Keyword, char, ICollection<Ability>> AbilitiesByTag { get; } =
            new TrieDictionary<Keyword, char, ICollection<Ability>>();

        private IDictionary<Ability, Ability.Context> RunningAbilities { get; } =
            new Dictionary<Ability, Ability.Context>();

        private EffectRegistry EffectRegistry { get; } = new EffectRegistry();

        [NotNull] 
        [field: SerializeField, Required]
        private Animator? Animator { get; set; }
        
        [NotNull] private KeywordContainer? KeywordContainer { get; set; }
        [NotNull] private AttributeSet? AttributeSet { get; set; }
        [NotNull] private IModifiable? ModifierConsumer { get; set; } 
        [NotNull] private AbilitySystemAnimationHandler? AnimationHandler { get; set; }
        [field: SerializeField] private List<Ability> DefaultAbilities { get; set; } = new List<Ability>();

        public event UnityAction<Ability> OnAbilityStarted = delegate { };
        public event UnityAction<Ability> OnAbilityStopped = delegate { };
        public event UnityAction<Ability> OnAbilityGranted = delegate { };
        public event UnityAction<Ability> OnAbilityRevoked = delegate { };

        protected override void Awake() {
            base.Awake();
            foreach (Ability ability in this.DefaultAbilities) {
                this.Grant(ability);
            }

            this.KeywordContainer = this.Root.GetOrAdd<KeywordContainer>();
            this.ModifierConsumer = this.AttributeSet = this.Root.GetOrAdd<AttributeSet>();
            this.AnimationController = AnimationController.Create(this.Animator);
            this.AnimationHandler = this.Animator.GetOrAddComponent<AbilitySystemAnimationHandler>();
            this.AnimationHandler.ConnectToAnimationController(this.AnimationController);
        }

        private void OnDestroy() {
            this.AnimationController?.Destroy();
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

        public void Revoke(Ability ability) {
            if (!this.AvailableAbilities.Remove(ability)) {
                return;
            }

            foreach (Keyword keyword in ability.Tags) {
                this.AbilitiesByTag[keyword].Remove(ability);
            }

            this.OnAbilityRevoked.Invoke(ability);
        }

        /// <summary>
        /// Attempts to execute the given ability. This will check if the ability system has the ability and
        /// the conditions for the ability to start are met.
        /// </summary>
        /// <param name="ability">The ability to perform.</param>
        /// <param name="userData">Optional user data for the ability.</param>
        /// <returns>An awaitable that completes when the ability has finished executing.</returns>
        public async Awaitable Perform(Ability? ability, IReadOnlyDictionary<string, double>? userData = null) {
            if (!ability) {
                return;
            }

            this.Stop(ability);
            if (!this.AvailableAbilities.Remove(ability)) {
                return;
            }

            if (!ability.TryCommit(this, userData, out Ability.Context context)) {
                return;
            }

            this.RunningAbilities[ability] = context;
            Awaitable execution = ability.Execute(context);
            this.OnAbilityStarted.Invoke(ability);
            await execution;
            
            this.RunningAbilities.Remove(ability);
            this.AvailableAbilities.Add(ability);
            this.OnAbilityStopped.Invoke(ability);
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
                await this.Perform(ability);
            }
        }

        /// <summary>
        /// Stops the given ability from executing.
        /// </summary>
        /// <param name="ability">The ability to stop.</param>
        public void Stop(Ability? ability) {
            if (!ability) {
                return;
            }

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

        public Awaitable<AnimationPlayResult> PlayAnimation(
            AnimationClip anim, CancellationToken interrupter,
            UnityAction<AnimationNotifier> onNotify, Action? onInterrupt = null
        ) {
            return this.AnimationController is null
                    ? AsyncTask<AnimationPlayResult>.FromResult(AnimationPlayResult.Invalid)
                    : this.AnimationController.PlayActionAnimation(anim, onNotify, onInterrupt, interrupter);
        }

        CancellationTokenSource IEffectReceiverFacade.Register(EffectDescriptor effect, CancellationToken interrupt) {
            return this.EffectRegistry.Register(effect, interrupt);
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
