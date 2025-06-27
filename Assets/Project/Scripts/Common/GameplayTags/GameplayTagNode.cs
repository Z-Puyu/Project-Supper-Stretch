using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Common.GameplayTags;

[Serializable]
public abstract class GameplayTagNode {
    [field: SerializeField, ReadOnly] public string Namespace { get; set; } = string.Empty;
    [field: SerializeField, ReadOnly] public string Path { get; set; } = string.Empty;
    
    [field: SerializeField, OnValueChanged(nameof(this.OnRename))] 
    public string Tag { get; private set; } = string.Empty;

    public string Name => $"{this.Namespace}.{this.Path}";

    public abstract IList<GameplayTagNode> Children { get; }
    public bool IsLeaf => this.Children.Count == 0;

    protected abstract void OnRename();
    
    public static implicit operator string(GameplayTagNode node) {
        return node.Name;   
    }
}
