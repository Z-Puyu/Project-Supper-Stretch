using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Scripts.Common.GameplayTags;

public class PreorderIterator<T> where T : GameplayTagNode {
    private Dictionary<T, T> Parents { get; init; } = [];
    private Stack<T> Stack { get; init; } = [];
    private T? Current { get; set; }
    public Action<T, T> ForEachChild { private get; set; } = delegate { };
    public Action<T> ForEach { private get; set; } = delegate { };
    public Action<T> AtStart { private get; set; } = delegate { };
    public Func<T, bool>? ShouldStop { private get; set; }
    public Action<T> OnEnd { private get; set; } = delegate { };
    
    public PreorderIterator(IList<T> firstLevel) {
        for (int i = firstLevel.Count - 1; i >= 0; i -= 1) {
            this.Stack.Push(firstLevel[i]);
        }
    }
    
    public bool AtEnd => this.Stack.Count(node => node is not null) == 0 ||
                         (this.Current is not null && (this.ShouldStop?.Invoke(this.Current) ?? false));

    public bool Start(out T? start) {
        if (!this.Stack.TryPeek(out T node)) {
            start = null;
            return false;
        }

        this.AtStart.Invoke(node);
        start = node;
        return true;
    }

    public T Next() {
        T next = this.Stack.Pop();
        
        for (int i = next.Children.Count - 1; i >= 0; i -= 1) {
            if (next.Children[i] is null) {
                continue;           
            }
            
            this.Parents.Add((T)next.Children[i], next);
            this.Stack.Push((T)next.Children[i]);       
        }

        return next;
    }
    
    public void Visit(T next) {
        this.ForEach.Invoke(next);
        this.Current = next;
        if (this.Parents.TryGetValue(next, out T parent)) {
            this.ForEachChild.Invoke(parent, next);
        }
    }

    public void Exit() {
        if (this.Current is not null) {
            this.OnEnd.Invoke(this.Current);
        }
    }
}
