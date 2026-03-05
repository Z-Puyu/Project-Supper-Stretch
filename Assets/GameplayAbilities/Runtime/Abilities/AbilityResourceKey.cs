using System;
using System.Linq;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [Serializable]
    public record struct AbilityResourceKey<T> {
        [field: SerializeField, Validate(nameof(AbilityResourceKey<T>.IsValidKey))] 
        private string Key { get; set; }
        
        internal bool IsEmpty => string.IsNullOrWhiteSpace(this.Key);

        internal bool IsSameKey(AbilityResourceKey<T> other) {
            return string.Equals(this.Key, other.Key, StringComparison.OrdinalIgnoreCase);
        }
        
        private static bool IsValidKey(string key) {
            return !string.IsNullOrWhiteSpace(key) && 
                   Ability.ExtractAllResourceKeys<T>().Count(k => k.Key == key) == 1;
        }
    }
}
