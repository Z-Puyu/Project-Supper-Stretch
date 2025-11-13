using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    [CreateAssetMenu(fileName = "New Modifier Effect", menuName = "Gameplay Abilities/Effects/Modifier Effect")]
    public class ModifierEffect : Effect {
        [field: SerializeField, Table]
        private List<EffectModifier> Modifiers { get; set; } = new List<EffectModifier>();
        
        public override void Apply(EffectSource source, EffectTarget target) {
            foreach (EffectModifier modifier in this.Modifiers) {
                target.AddModifier(modifier.CreateModifier(source, target));
            }
        }        
    }
}
