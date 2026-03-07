using System.Collections;
using System.Collections.Generic;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Attributes {
    [CreateAssetMenu(fileName = "Attribute Table", menuName = "Gameplay Abilities/Attribute Table")]
    internal sealed class AttributeTable : ScriptableObject, IEnumerable<KeyValuePair<GameplayAttributeType, double>> {
        [field: SerializeField, Dictionary("Attribute")]
        private Map<GameplayAttributeType, double> Entries { get; set; } = new Map<GameplayAttributeType, double>();

        public IEnumerator<KeyValuePair<GameplayAttributeType, double>> GetEnumerator() {
            return this.Entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
