using System;
using GameplayAbilities.Common;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [Serializable]
    public record struct AbilityExecutionContext {
        [field: SerializeField] internal Ability Ability { get; private set; }
        internal UserData? UserData { get; private set; }
        
        public AbilityExecutionContext(Ability ability, UserData? userData) {
            this.Ability = ability;
            this.UserData = userData;
        }
    }
}
