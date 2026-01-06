using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Components;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    [CreateAssetMenu(fileName = "New Attribute Table", menuName = "Gameplay Abilities/Attribute Table")]
    public sealed class AttributeTable : ScriptableObject, IEnumerable<KeyValuePair<AttributeType, double>>, IComponentInitialiser<AttributeSet> {
        [field: SerializeField, Table]
        private List<AttributeTableRow> BaseValues { get; set; } = new List<AttributeTableRow>();
        
        public IEnumerator<KeyValuePair<AttributeType, double>> GetEnumerator() {
            return this.BaseValues.Select(row => row.MakePair()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        
        public void Initialise(AttributeSet component) {
            if (!component) {
                Debug.LogError("Cannot initialise null AttributeSet", this);
                return;
            }
            
            component.Clear();
            foreach (KeyValuePair<AttributeType, double> entry in this) {
                component.Initialise(entry.Key, entry.Value);
            }
        }
    }
}