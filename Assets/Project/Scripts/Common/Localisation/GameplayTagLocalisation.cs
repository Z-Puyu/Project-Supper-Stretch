using System.Collections.Generic;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace Project.Scripts.Common.Localisation;

public abstract class GameplayTagLocalisation<S, T> : Localisation<T> where S : GameplayTagNode {
    [field: SerializeField, Required] private GameplayTagTree<S>? Source { get; set; }
    
    [Button]
    public override void Refetch() {
        if (!this.Source) {
            Logging.Warn("Nothing to fetch", this);
        } else {
            IDictionary<string, string> current = new Dictionary<string, string>(this.Entries);
            this.Entries.Clear();
            PreorderIterator<S> iterator = new PreorderIterator<S>(this.Source.Nodes);
            iterator.ForEach = node => this.Rewrite(node, current);
            this.Source.Traverse(iterator);
        }
    }

    protected abstract void Rewrite(S node, IDictionary<string, string> current);
}
