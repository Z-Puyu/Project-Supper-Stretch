using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    [Serializable]
    internal class AttributeTableRow {
        [field: SerializeField, TreeDropdown(nameof(this.AllAttributeTypes))]
        internal string Attribute { get; set; } = string.Empty;

        [field: SerializeField] internal double Value { get; set; }

        private AdvancedDropdownList<string> AllAttributeTypes => AttributeUtils.GetLeafAttributes();
        
        internal KeyValuePair<AttributeType, double> MakePair() {
            AttributeType? type = null;
            foreach (AttributeDefinitionSheet sheet in Database<AttributeDefinitionSheet>.LoadedResources) {
                type = sheet.Find(this.Attribute);
                if (type is not null) {
                    break;
                }
            }
            
            return new KeyValuePair<AttributeType, double>(type!, this.Value);
        }
    }
}
