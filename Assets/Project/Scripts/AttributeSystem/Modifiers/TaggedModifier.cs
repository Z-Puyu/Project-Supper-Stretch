using System;
using System.Collections.Generic;
using Project.Scripts.AttributeSystem.AttributeTypes;
using Project.Scripts.Util.Builder;
using Project.Scripts.Util.Visitor;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

/// <summary>
/// Base class for a tagged modifier. A tagged modifier only applies to attribute sets with the same tag.
/// </summary>
/// <typeparam name="K">The enum type representing the attribute modifiable by the modifier.</typeparam>
[Serializable]
public abstract class TaggedModifier<K> : Modifier<K> where K : Enum {
    [field: SerializeField]
    public string Tag { get; protected set; } = string.Empty;
    
    protected TaggedModifier(K target) : base(target) { }

    public override void Visit(ModifierManager manager) {
        manager.AddModifier(this.Tag, this);
    }
}