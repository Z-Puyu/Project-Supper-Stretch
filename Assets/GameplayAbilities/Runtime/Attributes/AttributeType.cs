using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilities.Attributes {
    [Serializable]
    public abstract class AttributeType : ScriptableObject {
        [field: SerializeField] public string Id { get; private set; } = string.Empty;
        [field: SerializeField] private string Name { get; set; } = string.Empty;
        
        public string DisplayName => string.IsNullOrWhiteSpace(this.Name) ? this.Id : this.Name;

        internal abstract IEnumerable<GameplayAttributeType> Resolve();
    }
}