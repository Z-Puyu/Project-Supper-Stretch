using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Common.GameplayTags;

[Serializable]
public class GameplayTag : GameplayTagNode {
    [field: SerializeReference] private List<GameplayTagNode> SubTags { get; set; } = [];

    public override IList<GameplayTagNode> Children => this.SubTags;

    protected override void OnRename() {
        this.TracePath<GameplayTagDefinition, GameplayTag>();
    }
}
