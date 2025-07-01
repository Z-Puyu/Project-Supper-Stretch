using System.Collections.Generic;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace Project.Scripts.Common.GameplayTags;

public abstract class GameplayTagLocalisationMapping<T> : ScriptableObject where T : GameplayTagNode {
    [field: SerializeField, Required] private GameplayTagTree<T>? Source { get; set; }
    
    [field: SerializeField, SaintsDictionary("Tag", "Localisation")]
    private SaintsDictionary<string, string> Entries { get; set; } = [];

    public string Map(T tag) {
        return this.Entries.GetValueOrDefault(tag.Name, tag.Tag);   
    }

    [Button]
    private void Refetch() {
        if (!this.Source) {
            Logging.Warn("Nothing to fetch", this);
        } else {
            IDictionary<string, string> current = new Dictionary<string, string>(this.Entries);
            this.Entries.Clear();
            PreorderIterator<T> iterator = new PreorderIterator<T>(this.Source.Nodes);
            iterator.ForEach = rewrite;
            this.Source.Traverse(iterator);
            
            void rewrite(T node) {
                this.Entries.Add(node.Name, current.TryGetValue(node.Name, out string? value) ? value : node.Tag);
            }
        }
    }
}
