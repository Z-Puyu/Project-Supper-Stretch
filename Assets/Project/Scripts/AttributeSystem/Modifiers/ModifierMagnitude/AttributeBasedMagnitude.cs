using System;
using Project.Scripts.AttributeSystem.Attributes;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers.ModifierMagnitude;

[Serializable]
public class AttributeBasedMagnitude<K> : Magnitude where K : Enum {
    private enum Source { Instigator, Target }
    
    [field: SerializeField]
    private Source BackingAttributeSource { get; set; } = Source.Instigator;
    
    [field: SerializeField]
    private K BackingAttribute { get; set; }
    
    private AttributeSet? BackingAttributeSet { get; init; }
    
    public override float Value => !this.BackingAttributeSet 
            ? 0 
            : this.BackingAttributeSet[this.BackingAttribute].CurrentValue;
    
    protected AttributeBasedMagnitude(K backingAttribute) {
        this.BackingAttribute = backingAttribute;
    }

    public override Magnitude BasedOn(AttributeSet? self, AttributeSet target) {
        AttributeSet? source = this.BackingAttributeSource switch {
            Source.Instigator => self,
            Source.Target => target,
            var _ => throw new ArgumentOutOfRangeException(nameof(this.BackingAttributeSource))
        };
        return !source ? this : new AttributeBasedMagnitude<K>(this.BackingAttribute) { BackingAttributeSet = source };
    }
}
