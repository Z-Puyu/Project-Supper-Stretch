using System;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes;

[Serializable]
public record ConstantValue : IValueProducer {
    [field: SerializeField] private int Value { get; set; }
    
    public int ProduceFrom(IAttributeReader target, IAttributeReader source) {
        return this.Value;
    }
}
