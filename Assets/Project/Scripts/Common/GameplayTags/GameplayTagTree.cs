using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Common.GameplayTags;

public abstract class GameplayTagTree<T> : ScriptableObject where T : GameplayTagNode {
    public static HashSet<GameplayTagTree<T>> Instances { get; } = [];
    
    [field: SerializeField, Required] public string Namespace { get; private set; } = string.Empty;
    [field: SerializeReference] public List<T> Nodes { get; private set; } = [];

    protected virtual void Awake() {
        GameplayTagTree<T>.Instances.Add(this);
        Logging.Info($"{GameplayTagTree<T>.Instances.Count} instances of {this.GetType().Name} found", this);
    }

    protected virtual void OnEnable() {
        GameplayTagTree<T>.Instances.Add(this);
        Logging.Info($"{GameplayTagTree<T>.Instances.Count} instances of {this.GetType().Name} found", this);
    }

    protected virtual void OnDisable() {
        GameplayTagTree<T>.Instances.Remove(this);
        Logging.Info($"{GameplayTagTree<T>.Instances.Count} instances of {this.GetType().Name} found", this);
    }

    protected virtual void OnDestroy() {
        GameplayTagTree<T>.Instances.Remove(this);
        Logging.Info($"{GameplayTagTree<T>.Instances.Count} instances of {this.GetType().Name} found", this);
    }

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

    public bool TryFind(string tag, out T? node) {
        T? res = null;
        PreorderIterator<T> iterator = new PreorderIterator<T>(this.Nodes);
        iterator.ShouldStop = n => n.Name == tag;
        iterator.OnEnd = n => res = n;
        this.Traverse(iterator);
        node = res;
        return res != null;       
    }
}
