using System;
using Attribute = Project.Scripts.AttributeSystem.Attributes.Attribute;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

public sealed record class CapturedAttributeData(Attributes.AttributeManagementSystem? Instigator, Attributes.AttributeManagementSystem Target) {
    private Attributes.AttributeManagementSystem? Instigator { get; init; } = Instigator;
    private Attributes.AttributeManagementSystem Target { get; init; } = Target;
    
    public Attribute ReadFromSource(Enum attribute) {
        return this.Instigator?.Query(attribute) ?? Attribute.Zero(attribute);
    }
    
    public Attribute ReadFromTarget(Enum attribute) {
        return this.Target.Query(attribute);
    }
}
