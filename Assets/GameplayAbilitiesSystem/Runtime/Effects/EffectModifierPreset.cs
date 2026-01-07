using System;
using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    [Serializable]
    internal sealed class EffectModifierPreset {
        [field: SerializeField, Table]
        private List<ModifierConfig> Modifiers { get; set; } = new List<ModifierConfig>();

        internal IEnumerable<Modifier> Apply(
            IEffectEmitterFacade source, IEffectReceiverFacade target, IReadOnlyDictionary<string, double>? userData
        ) {
            foreach (ModifierConfig modifierConfig in this.Modifiers) {
                if (modifierConfig.IsApplicable(source, target, userData, out Modifier modifier)) {
                    yield return modifier;
                }
            }
        }
    }
}
