using System;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    public record struct AnimationResource : IAbilityResource {
        [field: SerializeField] public AnimationClip Clip { get; private set; }
        
        public static implicit operator AnimationClip(AnimationResource resource) => resource.Clip;
    }
}
