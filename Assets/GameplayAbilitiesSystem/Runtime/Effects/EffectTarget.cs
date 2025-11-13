using System.Collections;
using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using GameplayKeywordsSystem.Runtime;
using UnityEngine;
using Attribute = GameplayAbilitiesSystem.Runtime.Attributes.Attribute;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AttributeSet), typeof(ModifierEnvironment), typeof(KeywordContainer))]
    public sealed class EffectTarget : MonoBehaviour, IAttributeReader, IModifiable {
        private AttributeSet AttributeSet { get; set; }
        private ModifierEnvironment ModifierEnvironment { get; set; }
        private KeywordContainer KeywordContainer { get; set; }
        
        private void Awake() {
            this.AttributeSet = this.GetComponent<AttributeSet>();
            this.ModifierEnvironment = this.GetComponent<ModifierEnvironment>();
            this.KeywordContainer = this.GetComponent<KeywordContainer>();
        }
        
        public double GetCurrent(AttributeKey key) {
            return this.AttributeSet.GetCurrent(key);
        }
        
        public bool Has(double threshold, AttributeKey key) {
            return this.AttributeSet.Has(threshold, key);
        }

        public IEnumerator<Attribute> GetEnumerator() {
            return this.AttributeSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void AddModifier(Modifier modifier) {
            this.ModifierEnvironment.AddModifier(modifier);
        }

        internal void Tag(Keyword keyword) {
            this.KeywordContainer.Add(keyword);
        }
        
        internal void Untag(Keyword keyword) {
            this.KeywordContainer.Remove(keyword);
        }
        
        internal bool HasTag(Keyword keyword) {
            return this.KeywordContainer.Contains(keyword);
        }
    }
}
