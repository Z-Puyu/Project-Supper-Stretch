using System;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    public record struct PrefabResource : IAbilityResource {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        
        public static implicit operator GameObject(PrefabResource resource) => resource.Prefab;
    }
}
