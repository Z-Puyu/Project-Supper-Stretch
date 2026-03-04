using System;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    public record struct IntegerResource : IAbilityResource {
        [field: SerializeField] public int Value { get; private set; }
        
        public static implicit operator int(IntegerResource resource) => resource.Value;
    }
}
