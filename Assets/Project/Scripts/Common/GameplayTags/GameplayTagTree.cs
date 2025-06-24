using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Common.GameplayTags;

public abstract class GameplayTagTree<T> : ScriptableObject where T : GameplayTagNode {
    [field: SerializeField, Required] public string Namespace { get; private set; } = string.Empty;
    [field: SerializeReference] public List<T> Nodes { get; private set; } = [];

    public void Traverse(PreorderIterator<T> iterator) {
        if (!iterator.Start(out T? _)) {
            return;
        }

        while (!iterator.AtEnd) {
            T next = iterator.Next();
            iterator.Visit(next);
        }

        iterator.Exit();
    }
}
