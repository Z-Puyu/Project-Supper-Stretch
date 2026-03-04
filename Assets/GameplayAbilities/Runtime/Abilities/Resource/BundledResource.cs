using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    public record struct BundledResource : IAbilityResource, IEnumerable<IAbilityResource> {
        [field: SerializeReference]
        public List<IAbilityResource> Resources { get; set; }
        
        public IEnumerator<IAbilityResource> GetEnumerator() {
            return this.Resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        
        public static implicit operator List<IAbilityResource>(BundledResource resource) => resource.Resources;
    }
}
