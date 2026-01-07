using System;
using System.Collections;
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
    public sealed class AbilitySystem : BehaviourComponent, IEffectEmitterFacade, IEffectReceiverFacade, IEnumerable<Ability> {
        private AnimationController? AnimationController { get; set; }
        private ICollection<Ability> AvailableAbilities { get; } = new HashSet<Ability>();

        private TrieDictionary<Keyword, char, ICollection<Ability>> AbilitiesByTag { get; } =
            new TrieDictionary<Keyword, char, ICollection<Ability>>();

        private IDictionary<Ability, AbilityActivation> RunningAbilities { get; } =
            new Dictionary<Ability, AbilityActivation>();

        private EffectRegistry EffectRegistry { get; } = new EffectRegistry();

        [NotNull] [field: SerializeField] private Animator? Animator { get; set; }
        [NotNull] private KeywordContainer? KeywordContainer { get; set; }
        [NotNull] private AttributeSet? AttributeSet { get; set; }
        [NotNull] private AbilitySystemAnimationHandler? AnimationHandler { get; set; }
        [field: SerializeField] private List<Ability> DefaultAbilities { get; set; } = new List<Ability>();

        public IAttributeReader AttributeReader => this.AttributeSet;
        public ITaggable<Keyword> EmitterKeywordContainer => this.KeywordContainer;
        IAttributeReader IEffectReceiverFacade.AttributeReader => this.AttributeSet;
        public IModifiable ModifierConsumer => this.AttributeSet;
        ITaggable<Keyword> IEffectReceiverFacade.ReceiverKeywordContainer => this.KeywordContainer;
        
        public event UnityAction<Ability> OnAbilityStarted = delegate { };
        public event UnityAction<Ability> OnAbilityStopped = delegate { };
        public event UnityAction<Ability> OnAbilityGranted = delegate { }; 
        public event UnityAction<Ability> OnAbilityRevoked = delegate { };

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
        public void Perform(Ability? ability, IReadOnlyDictionary<string, double>? userData = null) {
            if (!ability) {
                return;
            }

            this.Stop(ability);
            if (!this.AvailableAbilities.Remove(ability)) {
                return;
            }
            
            if (!ability.TryCommit(this, out AbilityActivation activation)) {
                return;
            }
            
            this.RunningAbilities[ability] = activation;
            _ = ability.Execute(this, userData, activation.Interrupter.Token);
            this.OnAbilityStarted.Invoke(ability);
        }

        /// <summary>
        /// Attempts to execute the first ability with the given keyword tag.
        /// </summary>
        /// <param name="keyword">The keyword tag to search for.</param>
        /// <param name="userData">Optional user data for the ability.</param>
        public void Perform(Keyword keyword, IReadOnlyDictionary<string, double>? userData = null) {
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
        public void Stop(Ability? ability) {
            if (!ability) {
                return;
            }
            
            if (!this.RunningAbilities.Remove(ability, out AbilityActivation activation)) {
                return;
            }

            activation.Interrupt();
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

        public async Awaitable PlayAnimation(
            AnimationClip anim, CancellationToken interrupter,
            UnityAction<AnimationNotifier> onNotify, Action? onInterrupt = null
        ) {
            if (this.AnimationController is null) {
                return;
            }
            
            await this.AnimationController.PlayActionAnimation(anim, onNotify, onInterrupt, interrupter);
        }
        
        CancellationTokenSource IEffectReceiverFacade.Register(EffectDescriptor effect, CancellationToken interrupt) {
            return this.EffectRegistry.Register(effect, interrupt);
        }
        
        public void StopEffects(Ability? ability, Effect? type, Keyword keyword) {
            this.EffectRegistry.Stop(ability, type, keyword);
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
