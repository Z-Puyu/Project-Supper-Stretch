using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Extensions;
using GameplayAbilitiesSystem.Runtime.Animations;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Effects;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    [DisallowMultipleComponent]
    public sealed class AbilitySystem : MonoBehaviour, IEffectEmitterFacade, IEffectReceiverFacade {
        private AnimationController? AnimationController { get; set; }
        private ICollection<Ability> AvailableAbilities { get; } = new HashSet<Ability>();
        private ICollection<Ability> RunningAbilities { get; } = new HashSet<Ability>();
    
        [NotNull]
        [field: SerializeField, Required] 
        public GameObject? Owner { get; private set; }
        
        [NotNull] [field: SerializeField] private Animator? Animator { get; set; }
        [NotNull] [field: SerializeField] private KeywordContainer? KeywordContainer { get; set; }
        [NotNull] [field: SerializeField] public AttributeSet? AttributeSet { get; private set; }
    
        private AbilitySystemAnimationHandler? AnimationHandler { get; set; }
    
        [field: SerializeField] private List<Ability> DefaultAbilities { get; set; } = new List<Ability>();
        
        public IModifiable ModifierConsumer => this.AttributeSet;
        public ITaggable<Keyword> KeywordConsumer => this.KeywordContainer;
        public IAttributeReader InstigatorAttributeSet => this.AttributeSet;
        public ICollection<Keyword> TagsOnEmitter => this.KeywordContainer;

        private void Awake() {
            this.DefaultAbilities.ForEach(this.AvailableAbilities.Add);
            this.DefaultAbilities.Clear();
            if (!this.Owner) {
                this.Owner = this.transform.root.gameObject;
            }
            
            if (!this.Animator) {
                this.Animator = this.Owner.TryGetComponentInChildren(out Animator animator)
                        ? animator
                        : this.Owner.AddComponent<Animator>();
            }

            if (!this.Animator.HasComponent<AbilitySystemAnimationHandler>()) {
                this.Animator.gameObject.AddComponent<AbilitySystemAnimationHandler>();
            }

            if (!this.KeywordContainer) {
                this.KeywordContainer = this.Owner.TryGetComponentInChildren(out KeywordContainer container)
                        ? container
                        : this.AddSubobject<KeywordContainer>("Keyword Container (auto-generated)");
            }
            
            if (!this.AttributeSet) {
                this.AttributeSet = this.Owner.TryGetComponentInChildren(out AttributeSet set)
                        ? set
                        : this.AddSubobject<AttributeSet>("Attribute Set (auto-generated)");
            }
        }

        private void OnEnable() {
            this.AnimationController = AnimationController.Create(this.Animator);
            if (this.AnimationHandler) {
                this.AnimationHandler.OnNotified += this.OnNotified;
            }
        }
        
        private void OnDisable() {
            if (this.AnimationHandler) {
                this.AnimationHandler.OnNotified -= this.OnNotified;
            }
        }

        private void OnNotified(AnimationNotifier notifier) {
            foreach (Ability ability in this.RunningAbilities) {
                ability.RespondToAnimationEvent(this, notifier);
            }
        }

        public void PerformAbility(Ability ability) {
            if (!this.AvailableAbilities.Contains(ability) || !ability.TryCommit()) {
                return;
            }
        
            ability.Execute(this);
        }

        public void PerformAction(AnimationClip clip) {
            this.AnimationController?.PlayActionAnimation(clip);
        }
    }
}
