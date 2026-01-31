using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Components;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Attributes {
    [CreateAssetMenu(fileName = "New Attribute Table", menuName = "Gameplay Abilities/Attribute Table")]
    internal sealed class AttributeTable : ScriptableObject,
                                           IEnumerable<KeyValuePair<AttributeType, double>>,
                                           IComponentInitialiser<AttributeSet> {
        [field: SerializeField, Table]
        private List<AttributeTableRow> BaseValues { get; set; } = new List<AttributeTableRow>();

        public IEnumerator<KeyValuePair<AttributeType, double>> GetEnumerator() {
            return this.BaseValues.Select(row => row.MakePair()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void Initialise(AttributeSet component) {
            component.Clear();
            List<KeyValuePair<AttributeType, double>> entries = this.OrderBy(row => row.Key).ToList();
            foreach (KeyValuePair<AttributeType, double> entry in entries) {
                component.Initialise(entry.Key, entry.Value);
            }
        }
    }
}