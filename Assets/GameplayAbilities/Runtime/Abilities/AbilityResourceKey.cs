using System;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [Serializable]
    public record struct AbilityResourceKey<T> {
        [field: SerializeField, Validate(nameof(AbilityResourceKey<T>.IsValidKey))] 
        private string Key { get; set; }
        
        internal bool IsEmpty => string.IsNullOrWhiteSpace(this.Key);

        private static bool IsValidKey(string key) {
            return !string.IsNullOrWhiteSpace(key);
        }
    }
}
