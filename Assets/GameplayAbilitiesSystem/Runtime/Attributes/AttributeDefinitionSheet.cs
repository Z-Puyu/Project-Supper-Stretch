using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    [CreateAssetMenu(fileName = "New Attribute Definitions", menuName = "Gameplay Abilities/Attribute Definitions")]
    internal sealed class AttributeDefinitionSheet : ScriptableObject {
        [field: SerializeField, DefaultExpand]
        private List<AttributeType> AttributeTypes { get; set; } = new List<AttributeType>();

        internal AttributeType? Find(string id) {
            foreach (AttributeType type in this.AttributeTypes) {
                if (type.HasChild(id, out AttributeType? child)) {
                    return child;
                }
            }
            
            return null;
        }
        
        internal IEnumerable<AdvancedDropdownList<string>> GetKeyDropdownLists() {
            return this.AttributeTypes.ConvertAll(type => type.ToAdvancedDropdownList());
        }

        private void OnValidate() {
            foreach (AttributeType type in this.AttributeTypes) {
                type?.Validate();
            }
        }
    }
}
