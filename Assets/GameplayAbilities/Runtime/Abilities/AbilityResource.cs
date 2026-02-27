using System;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [Serializable]
    public abstract class AbilityResource<T> : IAbilityResource {
        [field: SerializeField] public AbilityResourceKey<T> Key { get; set; }
        [field: SerializeField] public T? Value { get; set; }
    }
}
