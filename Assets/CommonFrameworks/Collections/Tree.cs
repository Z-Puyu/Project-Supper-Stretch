using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CommonFrameworks.Collections {
    public class Tree<T, S> : ITree<T, S> {
        public T Root { get; }
        public bool IsDirected { get; }
        private IDictionary<T, ISet<T>> AdjacencyList { get; } = new Dictionary<T, ISet<T>>();
        private IDictionary<T, S> VertexData { get; } = new Dictionary<T, S>();
        
        public S this[T vertex] { get => this.VertexData[vertex]; set => this.VertexData[vertex] = value; }
        public IEnumerable<S> Values => this.VertexData.Values;
        public IEnumerable<T> Vertices => this.AdjacencyList.Keys;

        private Tree(T root, bool directed) {
            this.Root = root;
            this.IsDirected = directed;
        }
        
        public static Tree<T, S> CreateUndirected(T root, IEnumerable<KeyValuePair<T, T>>? edges = null, IDictionary<T, S>? data = null) {
            return new Tree<T, S>(root, false).Fill(edges, data);
        }
        
        public static Tree<T, S> CreateDirected(T root, IEnumerable<KeyValuePair<T, T>>? edges = null, IDictionary<T, S>? data = null) {
            return new Tree<T, S>(root, true).Fill(edges, data);
        }

        public Tree<T, S> Fill(IEnumerable<KeyValuePair<T, T>>? edges = null, IDictionary<T, S>? data = null) {
            foreach ((T x, T y) in edges ?? Enumerable.Empty<KeyValuePair<T, T>>()) {
                this.Add(x);
                this.Add(y);
                this.Join(x, y);
            }
            
            foreach ((T vertex, S value) in data ?? Enumerable.Empty<KeyValuePair<T, S>>()) {
                this.Augment(vertex, value);
            }

            return this;
        }
        
        public bool ContainsVertex(T vertex) {
            return this.AdjacencyList.ContainsKey(vertex);
        }
        
        public bool Remove(T key) {
            this.VertexData.Remove(key);
            if (!this.AdjacencyList.Remove(key, out ISet<T> neighbourhood)) {
                return false;
            }

            if (this.IsDirected) {
                return true;
            }

            foreach (T neighbour in neighbourhood) {
                this.AdjacencyList[neighbour].Remove(key);
            }
            
            return true;
        }
        
        public bool TryGetValue(T key, [NotNullWhen(true)] out S? value) {
            return this.VertexData.TryGetValue(key, out value);
        }

        private C Convert<C>(T start, Func<T, C> converter, Func<T, IEnumerable<C>, C> combiner)
                where C : IEnumerable {
            return !this.AdjacencyList.TryGetValue(start, out ISet<T> children)
                    ? converter(start)
                    : combiner(start, children.Select(v => this.Convert(v, converter, combiner)));
        }

        public C Convert<C>(Func<T, C> converter, Func<T, IEnumerable<C>, C> combiner) where C : IEnumerable {
            return this.Convert(this.Root, converter, combiner);
        }

        public bool Join(T from, T to) {
            if (!this.AdjacencyList.TryGetValue(from, out ISet<T> neighbourhood) || !this.ContainsVertex(to)) {
                return false;
            }

            if (this.IsDirected) {
                return !this.ContainsEdge(to, from) && neighbourhood.Add(to);
            } 
            
            return neighbourhood.Add(to) && this.AdjacencyList[to].Add(from);
        }

        public bool ContainsEdge(T from, T to) {
            return this.AdjacencyList.TryGetValue(from, out ISet<T> neighbourhood) && neighbourhood.Contains(to);
        }

        public U Aggregate<U>(Func<T, S?, IEnumerable<U>, U> combiner, T source, Func<T, S?, U> synthesiser) {
            S? data = this.TryGetValue(source, out S? value) ? value : default;
            return this.AdjacencyList.TryGetValue(source, out ISet<T> children)
                    ? combiner(source, data, children.Select(v => this.Aggregate(combiner, v, synthesiser)))
                    : synthesiser(source, data);
        }

        public bool Add(T vertex) {
            if (this.AdjacencyList.ContainsKey(vertex)) {
                return false;
            }

            this.AdjacencyList.Add(vertex, new HashSet<T>());
            return true;
        }
        
        public bool Augment(T vertex, S data) {
            if (!this.ContainsVertex(vertex)) {
                return false;
            }
            
            this.VertexData[vertex] = data;
            return true;
        }
    }
}
