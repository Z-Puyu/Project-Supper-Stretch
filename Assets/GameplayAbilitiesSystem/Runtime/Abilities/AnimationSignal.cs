using System;
using GameplayAbilitiesSystem.Runtime.Abilities.Executions;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    [Serializable]
    internal sealed class AnimationSignal {
        [field: SerializeField, ReadOnly] internal string Name { get; private set; }
        
        [field: SerializeReference, ReferencePicker] 
        internal IAbilityExecutor? OnSignal { get; set; }
        
        public AnimationSignal(string name) {
            this.Name = name;
        }
    }
}
