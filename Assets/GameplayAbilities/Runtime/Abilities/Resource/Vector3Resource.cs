using System;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    internal record struct Vector3Resource : IAbilityResource {
        [field: SerializeField] public Vector3 Value { get; private set; }
        
        public static implicit operator Vector3(Vector3Resource resource) => resource.Value;
    }
}
