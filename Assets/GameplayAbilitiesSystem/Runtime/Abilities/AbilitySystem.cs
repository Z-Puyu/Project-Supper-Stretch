using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
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

        private IDictionary<Ability, CancellationTokenSource> RunningAbilities { get; } =
            new Dictionary<Ability, CancellationTokenSource>();

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

        /// <summary>
        /// Attempts to execute the given ability. This will check if the ability system has the ability and
        /// the conditions for the ability to start are met.
        /// </summary>
        /// <param name="ability"></param>
        public void Perform(Ability ability) {
            if (!this.AvailableAbilities.Remove(ability)) {
                return;
            }
            
            if (!ability.TryCommit(this)) {
                return;
            }

            CancellationTokenSource interrupter = new CancellationTokenSource();
            this.RunningAbilities[ability] = interrupter;
            ability.Execute(this, interrupter);
        }

        internal void Interrupt(Ability ability) {
            if (!this.RunningAbilities.Remove(ability, out CancellationTokenSource interrupter)) {
                return;
            }
            
            interrupter.Cancel();
            interrupter.Dispose();
            this.AvailableAbilities.Add(ability);
        }

        internal void Stop(Ability ability) {
            if (!this.RunningAbilities.Remove(ability, out CancellationTokenSource interrupter)) {
                return;
            }

            interrupter.Dispose();
            this.AvailableAbilities.Add(ability);
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
