using System;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers.ModifierMagnitude;

[Serializable]
public class AttributeBasedMagnitude<T, K> : Magnitude where T : Enum where K : Enum {
    private enum Source { Instigator, Target }
    
    [field: SerializeField]
    private Source BackingAttributeSource { get; set; } = Source.Instigator;
    
    [field: SerializeField]
    private string BackingAttributeSetTag { get; set; }
    
    [field: SerializeField]
    private K BackingAttribute { get; set; }
    
    protected AttributeBasedMagnitude(string backingAttributeSetTag, K backingAttribute) {
        this.BackingAttributeSetTag = backingAttributeSetTag;
        this.BackingAttribute = backingAttribute;
    }
    
    private Attributes.AttributeManagementSystem? BackingAttributeSystem { get; init; }
    
    public override float Evaluate() {
        return this.BackingAttributeSystem?.Query(this.BackingAttributeSetTag, this.BackingAttribute).CurrentValue ?? 0;
    }

    public override Magnitude BasedOn(Attributes.AttributeManagementSystem? self, Attributes.AttributeManagementSystem target) {
        Attributes.AttributeManagementSystem? source = this.BackingAttributeSource switch {
            Source.Instigator => self,
            Source.Target => target,
            var _ => throw new ArgumentOutOfRangeException(nameof(this.BackingAttributeSource))
        };
        return source == null
                ? this
                : new AttributeBasedMagnitude<T, K>(this.BackingAttributeSetTag, this.BackingAttribute) {
                    BackingAttributeSystem = source
                };
    }
}
