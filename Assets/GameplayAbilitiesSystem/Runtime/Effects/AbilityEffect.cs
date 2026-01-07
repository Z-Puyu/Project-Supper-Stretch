using System;
using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    [Serializable]
    internal sealed class AbilityEffect {
        [field: SerializeField] private EffectKeywordPreset KeywordPreset { get; set; } = new EffectKeywordPreset();
        [field: SerializeField] private EffectModifierPreset ModifierPreset { get; set; } = new EffectModifierPreset();
        private List<Modifier> AppliedModifiers { get; } = new List<Modifier>();
        
        public void Apply(AbilitySystem target, IReadOnlyDictionary<string, double>? userData) {
            this.KeywordPreset.Apply(target, target);
            this.AppliedModifiers.AddRange(this.ModifierPreset.Apply(target, target, userData));
            foreach (Modifier modifier in this.AppliedModifiers) {
                target.ModifierConsumer.AddModifier(modifier);
            }
        }
        
        public void Stop(AbilitySystem target) {
            foreach (Modifier modifier in this.AppliedModifiers) {
                target.ModifierConsumer.AddModifier(-modifier);
            }
            
            this.AppliedModifiers.Clear();
        }
    }
}
