using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    [DisallowMultipleComponent, RequireComponent(typeof(AttributeSet), typeof(ModifierEnvironment))]
    public sealed class EffectTarget : MonoBehaviour, IAttributeReader, IModifiable {
        private AttributeSet AttributeSet { get; set; }
        private ModifierEnvironment ModifierEnvironment { get; set; }
        
        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
            this.ModifierEnvironment = this.GetComponent<ModifierEnvironment>();
        }

        public double GetCurrent(AttributeKey key) {
            return this.AttributeSet.GetCurrent(key);
        }
        
        public bool Has(double threshold, AttributeKey key) {
            return this.AttributeSet.Has(threshold, key);
        }
        
        public void AddModifier(Modifier modifier) {
            this.ModifierEnvironment.AddModifier(modifier);
        }
    }
}
