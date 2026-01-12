using System;
using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    [Serializable]
    internal sealed class EffectModifierPreset {
        [field: SerializeField, FieldLabelText(nameof(this.LabelModifier), true)]
        private List<ModifierConfig> Modifiers { get; set; } = new List<ModifierConfig>();

        private string LabelModifier(ModifierConfig config) {
            return $"<b>{config}</b>";
        }

        internal IEnumerable<Modifier> Apply(
            IEffectEmitterFacade source, IEffectReceiverFacade target, IReadOnlyDictionary<string, double>? userData
        ) {
            foreach (ModifierConfig config in this.Modifiers) {
                yield return config.Instantiate(source, target, userData);
            }
        }
    }
}
