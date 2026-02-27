using System;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [Serializable]
    public record struct AbilityResourceKey<T> {
        [field: SerializeField] private string Key { get; set; }
        internal bool IsEmpty => string.IsNullOrWhiteSpace(this.Key);
    }
}
