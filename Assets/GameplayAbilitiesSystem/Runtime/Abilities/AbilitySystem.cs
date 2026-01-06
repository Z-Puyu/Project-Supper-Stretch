using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using CommonFrameworks.Collections;
using CommonFrameworks.Components;
using CommonFrameworks.Extensions;
using GameplayAbilitiesSystem.Runtime.Animations;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Effects;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using GameplayKeywordsSystem.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    [DisallowMultipleComponent]
    public sealed class AbilitySystem : BehaviourComponent, IEffectEmitterFacade, IEffectReceiverFacade {
        private AnimationController? AnimationController { get; set; }
        private ICollection<Ability> AvailableAbilities { get; } = new HashSet<Ability>();

        private TrieDictionary<Keyword, char, ICollection<Ability>> AbilitiesByTag { get; } =
            new TrieDictionary<Keyword, char, ICollection<Ability>>();

        private IDictionary<Ability, AbilityActivation> RunningAbilities { get; } =
            new Dictionary<Ability, AbilityActivation>();

        private EffectRegistry EffectRegistry { get; } = new EffectRegistry();

        [NotNull] [field: SerializeField] private Animator? Animator { get; set; }
        [NotNull] [field: SerializeField] private KeywordContainer? KeywordContainer { get; set; }
        [NotNull] [field: SerializeField] public AttributeSet? AttributeSet { get; private set; }
        [NotNull] private AbilitySystemAnimationHandler? AnimationHandler { get; set; }
        [field: SerializeField] private List<Ability> DefaultAbilities { get; set; } = new List<Ability>();

        IAttributeReader IEffectEmitterFacade.AttributeReader => this.AttributeSet;
        public ITaggable<Keyword> EmitterKeywordContainer => this.KeywordContainer;
        IAttributeReader IEffectReceiverFacade.AttributeReader => this.AttributeSet;
        IModifiable IEffectReceiverFacade.ModifierConsumer => this.AttributeSet;
        ITaggable<Keyword> IEffectReceiverFacade.ReceiverKeywordContainer => this.KeywordContainer;

        protected override void Awake() {
            base.Awake();
            foreach (Ability ability in this.DefaultAbilities) {
                this.Grant(ability);
            }
            
            if (!this.Animator) {
                this.Animator = this.Owner.TryGetComponentInChildren(out Animator animator)
                        ? animator
                        : this.Owner.AddComponent<Animator>();
            }

            this.KeywordContainer = this.Root.GetOrAdd<KeywordContainer>();
            this.AttributeSet = this.Root.GetOrAdd<AttributeSet>();
            this.AnimationController = AnimationController.Create(this.Animator);
            this.AnimationHandler = this.Animator.GetOrAddComponent<AbilitySystemAnimationHandler>();
            this.AnimationHandler.ConnectToAnimationController(this.AnimationController);
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
            
            return true;
        }

        /// <summary>
        /// Attempts to execute the given ability. This will check if the ability system has the ability and
        /// the conditions for the ability to start are met.
        /// </summary>
        /// <param name="ability">The ability to perform.</param>
        public void Perform(Ability ability) {
            if (!this.AvailableAbilities.Remove(ability)) {
                return;
            }
            
            if (!ability.TryCommit(this, out AbilityActivation activation)) {
                return;
            }
            
            this.RunningAbilities[ability] = activation;
            ability.Execute(this, activation.Interrupter.Token);
        }

        /// <summary>
        /// Attempts to execute the first ability with the given keyword tag.
        /// </summary>
        /// <param name="keyword">The keyword tag to search for.</param>
        public void Perform(Keyword keyword) {
            Ability? ability = this.AbilitiesByTag
                                   .DepthFirstPrefixSearch(keyword.Value)
                                   .FirstOrDefault().Value.FirstOrDefault();
            if (!ability) {
                return;
            }
            
            this.Perform(ability);
        }

        /// <summary>
        /// Stops the given ability from executing.
        /// </summary>
        /// <param name="ability">The ability to stop.</param>
        public void Stop(Ability ability) {
            if (!this.RunningAbilities.Remove(ability, out AbilityActivation activation)) {
                return;
            }

            activation.Stop(this);
            this.AvailableAbilities.Add(ability);
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

        public async Awaitable PlayAnimation(
            AnimationClip anim, CancellationToken interrupter, 
            UnityAction<AnimationNotifier> onNotify, Action? onInterrupt = null
        ) {
            if (this.AnimationController is null) {
                return;
            }
            
            await this.AnimationController.PlayActionAnimation(anim, onNotify, onInterrupt, interrupter);
        }

        internal void RegisterRunningEffect(ContinuousEffect effect, Ability sourceAbility) {
            this.EffectRegistry.RegisterEffect(effect);
        }

        public void StopEffects(Ability? ability = null, Effect? type = null, Keyword keyword = default) {
            this.EffectRegistry.StopEffects(ability, type, keyword);
        }
    }
}
