using System.Diagnostics.CodeAnalysis;
using GameplayAbilities.Attributes;
using GameplayAbilities.Effects;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    /// <summary>
    /// A facade class to control an <see cref="AbilitySystem"/>.
    /// </summary>
    public sealed class AbilitySystemController {
        public GameObject Owner { get; init; }
        private AbilitySystem AbilitySystem { get; init; }
        private EffectReceiver EffectReceiver { get; init; }
        private RuntimeAbilityResourceContainer ResourceContainer { get; init; }
        private IAttributeReader AttributeReader { get; init; }
        private Animator Animator { get; init; }

        internal AbilitySystemController(
            GameObject owner, AbilitySystem abilitySystem, EffectReceiver effectReceiver,
            RuntimeAbilityResourceContainer resourceContainer, IAttributeReader attributeReader, Animator animator
        ) {
            this.Owner = owner;
            this.AbilitySystem = abilitySystem;
            this.EffectReceiver = effectReceiver;
            this.ResourceContainer = resourceContainer;
            this.AttributeReader = attributeReader;
            this.Animator = animator;
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
        
        /// <summary>
        /// Tries to get the ability resource with the given key.
        /// </summary>
        /// <param name="key">The key of the resource to get.</param>
        /// <param name="resource">The resource to get.</param>
        /// <typeparam name="T">The type of the resource to get.</typeparam>
        /// <returns><c>true</c> if the resource was found; otherwise, <c>false</c>.</returns>
        public bool HasAbilityResource<T>(AbilityResourceKey<T> key, [NotNullWhen(true)] out T? resource)
                where T : IAbilityResource {
            return this.ResourceContainer.HasResource(key, out resource);
        }
    }
}
