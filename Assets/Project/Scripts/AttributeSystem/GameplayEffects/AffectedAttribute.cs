using System;
using Project.Scripts.AttributeSystem.AttributeTypes;
using Project.Scripts.AttributeSystem.Modifiers;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

[Serializable]
public abstract class AffectedAttribute {
    public abstract Enum EnumAttribute { get; }
    
    [field: SerializeField]
    public string AttributeSetTag { get; protected set; } = string.Empty;
    
    [field: SerializeField]
    public ModifierType ModifierType { get; private set; }

    [field: SerializeField]
    public string Label { get; private set; } = string.Empty;
}

[Serializable]
public abstract class AffectedAttribute<K> : AffectedAttribute where K : Enum {
    [field: SerializeField]
    private K Attribute { get; set; }

    public override Enum EnumAttribute => this.Attribute;
    
    private AffectedAttribute(K attribute) {
        this.Attribute = attribute;
    }
}
