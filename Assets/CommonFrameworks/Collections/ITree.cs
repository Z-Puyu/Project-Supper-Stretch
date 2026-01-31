using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonFrameworks.Collections {
    public interface ITree<T, S> {
        public T Root { get; }
        public S this[T vertex] { get; set; }
        
        public bool Join(T from, T to);
        public bool Add(T vertex);
        public bool Augment(T vertex, S data);
        public bool ContainsVertex(T vertex);
        public bool ContainsEdge(T from, T to);

        public U Aggregate<U>(Func<T, S?, IEnumerable<U>, U> combiner, T source, Func<T, S?, U> synthesiser);
    }
}
