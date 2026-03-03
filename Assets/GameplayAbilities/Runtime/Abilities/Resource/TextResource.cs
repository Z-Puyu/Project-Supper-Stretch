using System;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    internal record struct TextResource : IAbilityResource {
        [field: SerializeField] public string Text { get; private set; }
        
        public static implicit operator string(TextResource resource) => resource.Text;
    }
}
