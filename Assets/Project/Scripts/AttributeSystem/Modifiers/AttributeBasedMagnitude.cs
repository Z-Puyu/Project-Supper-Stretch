using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

public class AttributeBasedMagnitude<K> : Magnitude where K : Enum {
    private enum Source { Self, Target }
    
    [field: SerializeField]
    private Source BackingAttributeSource { get; set; } = Source.Self;
    
    [field: SerializeField]
    private K BackingAttribute { get; set; }
    
    protected AttributeBasedMagnitude(K backingAttribute) {
        this.BackingAttribute = backingAttribute;
    }
    
    private Attributes.AttributeManagementSystem? BackingAttributeSystem { get; init; }
    
    public override float Evaluate() {
        return this.BackingAttributeSystem?.Query(this.BackingAttribute).CurrentValue ?? 0;
    }

    public override float Evaluate(Enum tag) {
        return this.BackingAttributeSystem?.Query(tag, this.BackingAttribute).CurrentValue ?? 0;
    }

    public override Magnitude BasedOn(Attributes.AttributeManagementSystem? self, Attributes.AttributeManagementSystem target) {
        Attributes.AttributeManagementSystem? source = this.BackingAttributeSource switch {
            Source.Self => self,
            Source.Target => target,
            var _ => throw new ArgumentOutOfRangeException(nameof(this.BackingAttributeSource))
        };
        return source == null
                ? this
                : new AttributeBasedMagnitude<K>(this.BackingAttribute) { BackingAttributeSystem = source };
    }
}
