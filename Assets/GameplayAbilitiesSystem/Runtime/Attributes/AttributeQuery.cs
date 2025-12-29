using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    internal struct AttributeQuery {
        internal GameObject SourceObject { get; }
        internal IAttributeReader Source { get; }
        internal AttributeKey Id { get; }
        internal double Value { get; set; }
        internal bool IsValueApproximated { get; set; }
        
        internal AttributeQuery(GameObject sourceObject, IAttributeReader source, AttributeKey id, double value, bool isValueApproximated) {
            this.SourceObject = sourceObject;
            this.Source = source;
            this.Id = id;
            this.Value = value;
            this.IsValueApproximated = isValueApproximated;
        }
    }
}