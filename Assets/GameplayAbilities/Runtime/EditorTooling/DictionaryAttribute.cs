using System;
using UnityEngine;

namespace GameplayAbilities.Runtime.EditorTooling {
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DictionaryAttribute : PropertyAttribute {
        public string KeyLabel { get; }
        public string ValueLabel { get; }
        
        public DictionaryAttribute(string keyLabel = "", string valueLabel = "") {
            this.KeyLabel = keyLabel;
            this.ValueLabel = valueLabel;
        }
    }
}
