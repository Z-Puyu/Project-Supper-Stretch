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
/// <typeparam name="T">The tag type.</typeparam>
/// <typeparam name="K">The enum type representing the attribute modifiable by the modifier.</typeparam>
[Serializable]
public abstract class TaggedModifier<T, K> : Modifier<K> where T : Enum where K : Enum {
    [field: SerializeField]
    public T Tag { get; protected set; }

    protected TaggedModifier(T tag, K target) : base(target) {
        this.Tag = tag;
    }

    public override void Visit(ModifierManager manager) {
        manager.AddModifier(this.Tag, this);
    }
}