using System;
using UnityEngine;

namespace GameplayAbilities.Abilities.Resource {
    [Serializable]
    public record struct TextResource : IAbilityResource {
        [field: SerializeField, TextArea] public string Text { get; private set; }
        
        public static implicit operator string(TextResource resource) => resource.Text;
    }
}
