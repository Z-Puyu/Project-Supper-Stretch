using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    [Serializable]
    internal class AttributeTableRow {
        [field: SerializeField, TreeDropdown(nameof(this.AllAttributeTypes))]
        internal AttributeType Attribute { get; set; }

        [field: SerializeField] internal double Value { get; set; }

        private AdvancedDropdownList<AttributeType> AllAttributeTypes => AttributeUtils.GetLeafTypes();
        
        internal KeyValuePair<AttributeType, double> MakePair() {
            return new KeyValuePair<AttributeType, double>(this.Attribute, this.Value);
        }
    }
}
