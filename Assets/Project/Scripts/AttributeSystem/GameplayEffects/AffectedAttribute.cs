using System;
using Project.Scripts.AttributeSystem.AttributeTypes;
using Project.Scripts.AttributeSystem.Modifiers;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

[Serializable]
public abstract class AffectedAttribute {
    public abstract Enum EnumAttribute { get; }
    public abstract Enum EnumAttributeSetTag { get; }
    
    [field: SerializeField]
    public ModifierType ModifierType { get; private set; }

    [field: SerializeField]
    public string Label { get; private set; } = string.Empty;
}

[Serializable]
public abstract class AffectedAttribute<T, K> where T : Enum where K : Enum {
    [field: SerializeField]
    private K Attribute { get; set; }

    [field: SerializeField]
    private T AttributeSetTag { get; set; }

    public Enum EnumAttribute => this.Attribute;
    public Enum EnumAttributeSetTag => this.AttributeSetTag;
    
    public AffectedAttribute(K attribute, T attributeSetTag) {
        this.Attribute = attribute;
        this.AttributeSetTag = attributeSetTag;
    }
}
