using System;
using GameplayAbilities.Effects;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    public record struct EffectResource : IAbilityResource {
        [field: SerializeField] public Effect Effect { get; private set; }
        
        public static implicit operator Effect(EffectResource resource) => resource.Effect;
    }
}
