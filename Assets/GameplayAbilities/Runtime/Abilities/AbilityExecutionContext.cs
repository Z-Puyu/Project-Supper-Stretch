using System;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [Serializable]
    public record struct AbilityExecutionContext {
        [field: SerializeField] internal Ability Ability { get; set; }
        [field: SerializeField] internal AbilityExecutionUserData? UserData { get; set; }
        
        public AbilityExecutionContext(Ability ability, AbilityExecutionUserData? userData) {
            this.Ability = ability;
            this.UserData = userData;
        }
    }
}
