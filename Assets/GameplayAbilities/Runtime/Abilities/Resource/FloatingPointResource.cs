using System;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    public record struct FloatingPointResource : IAbilityResource {
        [field: SerializeField] public float Value { get; private set; }
        
        public static implicit operator float(FloatingPointResource resource) => resource.Value;
    }
}
