using System;
using System.Collections.Generic;
using GameplayAbilities.Attributes;
using GameplayAbilities.Common;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Effects {
    [Serializable]
    internal sealed class EffectModifierPreset {
        [field: SerializeField] private List<ModifierConfig> Modifiers { get; set; } = new List<ModifierConfig>();

        private string LabelModifier(ModifierConfig config) {
            return $"<b>{config}</b>";
        }

        internal IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> Apply(
            IAttributeReader source, IAttributeReader target, IUserData? userData = null
        ) {
            foreach (ModifierConfig config in this.Modifiers) {
                if (!config.Target) {
                    continue;
                }
                
                Modifier modifier = config.Instantiate(source, target, userData);
                foreach (GameplayAttributeType t in config.Target.Resolve()) {
                    yield return new KeyValuePair<GameplayAttributeType, Modifier>(t, modifier);
                }
            }
        }
    }
}
