using System;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    public record struct Vector2Resource : IAbilityResource {
        [field: SerializeField] public Vector2 Value { get; private set; }
        
        public static implicit operator Vector2(Vector2Resource resource) => resource.Value;
    }
}
