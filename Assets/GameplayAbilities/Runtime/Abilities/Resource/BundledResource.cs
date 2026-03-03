using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    internal sealed class BundledResource : IAbilityResource {
        [field: SerializeReference]
        public List<IAbilityResource> Resources { get; set; } = new List<IAbilityResource>();
        
        public static implicit operator List<IAbilityResource>(BundledResource resource) => resource.Resources;
    }
}
