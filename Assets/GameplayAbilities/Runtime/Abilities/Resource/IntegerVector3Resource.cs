using System;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    internal record struct IntegerVector3Resource : IAbilityResource {
        [field: SerializeField] public Vector3Int Value { get; private set; }
        
        public static implicit operator Vector3Int(IntegerVector3Resource resource) => resource.Value;
    }
}
