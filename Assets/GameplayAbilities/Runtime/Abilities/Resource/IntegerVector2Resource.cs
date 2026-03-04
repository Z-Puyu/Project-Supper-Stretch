using System;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    public record struct IntegerVector2Resource : IAbilityResource {
        [field: SerializeField] public Vector2Int Value { get; private set; }
        
        public static implicit operator Vector2Int(IntegerVector2Resource resource) => resource.Value;
    }
}
