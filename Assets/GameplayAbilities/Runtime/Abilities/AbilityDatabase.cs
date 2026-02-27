using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    internal static class AbilityDatabase {
        private static IEnumerable<AbilityResourceKey<T>> ExtractResourceKeys<T>() {
            return Resources.LoadAll<Ability>(string.Empty).SelectMany(ability => ability.ExtractResourceKeys<T>());
        }
    }
}
