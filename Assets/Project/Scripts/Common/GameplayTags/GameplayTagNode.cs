using System;
using System.Collections.Generic;
using Editor;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Common.GameplayTags;

[Serializable]
public abstract class GameplayTagNode : IEquatable<GameplayTagNode> {
    [field: SerializeField, ReadOnly] public string Namespace { get; set; } = string.Empty;
    [field: SerializeField, ReadOnly] public string Path { get; set; } = string.Empty;
    
    [field: SerializeField, OnValueChanged(nameof(this.OnRename))] 
    public string Tag { get; private set; } = string.Empty;

    public string Name => $"{this.Namespace}.{this.Path}";

    public abstract IList<GameplayTagNode> Children { get; }
    public bool IsLeaf => this.Children.Count == 0;

    protected abstract void OnRename();

    public bool Equals(GameplayTagNode? other) {
        if (other is null) {
            return false;
        }

        if (object.ReferenceEquals(this, other)) {
            return true;
        }

        return this.Namespace == other.Namespace && this.Path == other.Path;
    }
    
    public static implicit operator string(GameplayTagNode node) {
        return node.Name;   
    }
}
