using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilities.Attributes {
    [CreateAssetMenu(menuName = "Gameplay Abilities/Attributes/Virtual Attribute Type")]
    internal class VirtualAttributeType : AttributeType {
        [field: SerializeField]
        private List<GameplayAttributeType> BundledAttributeTypes { get; set; } = new List<GameplayAttributeType>();
        
        internal override IEnumerable<GameplayAttributeType> Resolve() {
            return this.BundledAttributeTypes;
        }
    }
}
