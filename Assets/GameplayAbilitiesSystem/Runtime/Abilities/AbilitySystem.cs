using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Collections;
using CommonFrameworks.Components;
using CommonFrameworks.Extensions;
using GameplayAbilitiesSystem.Runtime.Animations;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Effects;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using GameplayKeywordsSystem.Runtime;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    [DisallowMultipleComponent]
    public sealed class AbilitySystem : BehaviourComponent, IEffectEmitterFacade, IEffectReceiverFacade {
        private AnimationController? AnimationController { get; set; }
        private ICollection<Ability> AvailableAbilities { get; } = new HashSet<Ability>();

        private Multimap<Ability, ContinuousEffect, HashSet<ContinuousEffect>> RunningAbilities { get; } =
            new Multimap<Ability, ContinuousEffect, HashSet<ContinuousEffect>>();
        
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
            this.DefaultAbilities.ForEach(this.AvailableAbilities.Add);
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
        
        private void OnEnable() { 
            this.AnimationHandler.OnNotified += this.OnNotified;
        }
        
        private void OnDisable() {
            this.AnimationHandler.OnNotified -= this.OnNotified;
        }

        private void OnNotified(AnimationNotifier notifier) {
            foreach (Ability ability in this.RunningAbilities.Keys) {
                ability.RespondToAnimationEvent(this, notifier);
            }
        }

        public void PerformAbility(Ability ability) {
            if (!this.AvailableAbilities.Contains(ability) || !ability.TryCommit(this)) {
                return;
            }
        
            ability.Execute(this);
        }

        public void PlayAnimation(AnimationClip clip) {
            this.StartCoroutine(this.AnimationController?.PlayActionAnimation(clip));
        }

        public void StopEffects(
            Ability? sourceAbility = null, Effect? sourceEffect = null, Keyword sourceKeyword = default
        ) {
            EffectDescriptor descriptor = new EffectDescriptor(sourceEffect, sourceKeyword);
            if (sourceAbility) {
                this.RunningAbilities.Remove(sourceAbility, descriptor.Matches);
            } else {
                this.RunningAbilities.Remove(descriptor.Matches);
            }
        }
        
        internal void RegisterRunningEffect(ContinuousEffect effect, Ability sourceAbility) {
            this.RunningAbilities.Add(sourceAbility, effect);
        }
    }
}
