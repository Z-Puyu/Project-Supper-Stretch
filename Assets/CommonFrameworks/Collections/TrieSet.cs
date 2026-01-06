using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CommonFrameworks.Collections {
    public sealed class TrieSet<K, T> : ITrie<K, T>, ISet<K> where K : IEnumerable<T> {
        private sealed class Node {
            internal IDictionary<T, Node> Children { get; } = new Dictionary<T, Node>();
            internal bool IsEndOfKey { get; set; }
            internal int Size { get; set; }
            internal K Key { get; set; } = default!;
        }

        private Node Root { get; } = new Node();
        private T Separator { get; } = default!;
        private bool HasSeparator { get; }

        public TrieSet() {
            this.HasSeparator = false;
        }

        public TrieSet(T separator) {
            this.Separator = separator;
            this.HasSeparator = true;
        }

        #region Collection Semantics

        public int Count => this.Root.Size;
        public bool IsReadOnly => false;

        public IEnumerator<K> GetEnumerator() {
            return this.BreathFirstPrefixSearch(Enumerable.Empty<T>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        void ICollection<K>.Add(K item) {
            this.Add(item);
        }

        public void Clear() {
            this.Root.Children.Clear();
            this.Root.Size = 0;
        }

        public bool Contains(K item) {
            return this.HasPath(item, out List<(Node node, T element)> path) && path[^1].node.IsEndOfKey;
        }

        public void CopyTo(K[] array, int arrayIndex) {
            this.BreathFirstPrefixSearch(Enumerable.Empty<T>()).ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(K key) {
            if (!this.HasPath(key, out List<(Node node, T element)> path) || path.Count == 0) {
                return false;
            }

            if (!path[^1].node.IsEndOfKey) {
                return false;
            }

            path[^1].node.IsEndOfKey = false;
            this.Root.Size -= 1;
            for (int i = 1; i < path.Count; i += 1) {
                path[i].node.Size -= 1;
                if (path[i].node.Size > 0) {
                    continue;
                }

                path[i - 1].node.Children.Remove(path[i].element);
                break;
            }

            return true;
        }

        #endregion

        #region Set Semantics

        public bool Add(K item) {
            List<Node> path = new List<Node>();
            Node curr = this.Root;
            path.Add(curr);
            foreach (T element in item) {
                if (!curr.Children.TryGetValue(element, out Node node)) {
                    node = new Node();
                    curr.Children.Add(element, node);
                }

                curr = node;
                path.Add(curr);
            }

            if (curr.IsEndOfKey) {
                return false;
            }

            curr.IsEndOfKey = true;
            curr.Key = item;
            foreach (Node node in path) {
                node.Size += 1;
            }

            return true;
        }

        public void ExceptWith(IEnumerable<K> other) {
            ISet<K> set = other.ToHashSet();
            foreach (K key in this.Where(key => set.Contains(key))) {
                this.Remove(key);
            }
        }

        public void IntersectWith(IEnumerable<K> other) {
            ISet<K> set = other.ToHashSet();
            foreach (K key in this.Where(key => !set.Contains(key))) {
                this.Remove(key);
            }
        }

        public bool IsProperSubsetOf(IEnumerable<K> other) {
            return other.ToHashSet().IsProperSupersetOf(this);
        }

        public bool IsProperSupersetOf(IEnumerable<K> other) {
            ISet<K> set = other.ToHashSet();
            return this.Count > set.Count && this.IsSupersetOf(set);
        }

        public bool IsSubsetOf(IEnumerable<K> other) {
            return other.ToHashSet().IsSupersetOf(this);
        }

        public bool IsSupersetOf(IEnumerable<K> other) {
            return other.All(this.Contains);
        }

        public bool Overlaps(IEnumerable<K> other) {
            return other.Any(this.Contains);
        }

        public bool SetEquals(IEnumerable<K> other) {
            ISet<K> set = other.ToHashSet();
            return this.IsSubsetOf(set) && set.IsSubsetOf(this);
        }

        public void SymmetricExceptWith(IEnumerable<K> other) {
            ISet<K> set = other.ToHashSet();
            foreach (K key in this.Where(key => set.Contains(key))) {
                this.Remove(key);
            }

            foreach (K key in set.Where(key => !this.Contains(key))) {
                this.Add(key);
            }
        }

        public void UnionWith(IEnumerable<K> other) {
            foreach (K key in other) {
                this.Add(key);
            }
        }

        #endregion

        private bool HasPath<P>(P prefix, out List<(Node node, T element)> path) where P : IEnumerable<T> {
            path = new List<(Node node, T element)> { (this.Root, default!) };
            foreach (T element in prefix) {
                if (!path[^1].node.Children.TryGetValue(element, out Node node)) {
                    return false;
                }

                path.Add((node, element));
            }

            return !this.HasSeparator || path[^1].node.Children.ContainsKey(this.Separator);
        }

        public bool ContainsPrefix<P>(P prefix) where P : IEnumerable<T> {
            return this.HasPath(prefix, out List<(Node node, T element)> _);
        }

        public bool ContainsPrefixKey<S>(S sequence, [NotNullWhen(true)] out K? key) where S : IEnumerable<T> {
            Node curr = this.Root;
            foreach (T element in sequence) {
                if (curr.IsEndOfKey) {
                    key = curr.Key;
                    return true;
                }

                if (curr.Children.TryGetValue(element, out curr)) {
                    continue;
                }

                key = default;
                return false;
            }

            key = default;
            return false;
        }

        public bool FindLongestPrefixKey<S>(S sequence, [NotNullWhen(true)] out K? key) where S : IEnumerable<T> {
            Node curr = this.Root;
            bool found = false;
            key = default;
            foreach (T element in sequence) {
                if (curr.IsEndOfKey) {
                    found = true;
                    key = curr.Key;
                } else if (!curr.Children.TryGetValue(element, out curr)) {
                    break;
                }
            }

            return found;
        }

        public IList<K> BreathFirstPrefixSearch<P>(P prefix) where P : IEnumerable<T> {
            if (!this.HasPath(prefix, out List<(Node node, T element)> path)) {
                return new List<K>();
            }

            List<K> keys = new List<K>();
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(path[^1].node);
            while (queue.TryDequeue(out Node curr)) {
                if (curr.IsEndOfKey) {
                    keys.Add(curr.Key);
                } else {
                    foreach (Node node in curr.Children.Values) {
                        queue.Enqueue(node);
                    }
                }
            }

            return keys;
        }

        public IList<K> DepthFirstPrefixSearch<P>(P prefix) where P : IEnumerable<T> {
            if (!this.HasPath(prefix, out List<(Node node, T element)> path)) {
                return new List<K>();
            }

            List<K> keys = new List<K>();
            Stack<Node> stack = new Stack<Node>();
            stack.Push(path[^1].node);
            while (stack.TryPop(out Node curr)) {
                if (curr.IsEndOfKey) {
                    keys.Add(curr.Key);
                } else {
                    foreach (Node node in curr.Children.Values) {
                        stack.Push(node);
                    }
                }
            }

            return keys;
        }

        public bool RemoveAllWithPrefix<P>(P prefix) where P : IEnumerable<T> {
            if (!this.HasPath(prefix, out List<(Node node, T element)> path)) {
                return false;
            }

            path[^1].node.Children.Clear();
            path[^1].node.IsEndOfKey = false;
            int size = path[^1].node.Size;
            for (int i = 1; i < path.Count; i += 1) {
                path[i].node.Size -= size;
                if (path[i].node.Size > 0) {
                    continue;
                }

                path[i - 1].node.Children.Remove(path[i].element);
                break;
            }

            this.Root.Size -= size;
            return true;
        }

        public bool RemoveAllWithPrefix<P>(P prefix, out IEnumerable<K> removed) where P : IEnumerable<T> {
            if (!this.HasPath(prefix, out List<(Node node, T element)> path)) {
                removed = Enumerable.Empty<K>();
                return false;
            }

            IList<K> removedKeys = this.BreathFirstPrefixSearch(path.Select(p => p.element));
            removed = removedKeys;
            if (removedKeys.Count == 0) {
                return false;
            }

            for (int i = 1; i < path.Count; i += 1) {
                path[i].node.Size -= removedKeys.Count;
                if (path[i].node.Size > 0) {
                    continue;
                }

                path[i - 1].node.Children.Remove(path[i].element);
                break;
            }

            this.Root.Size -= removedKeys.Count;
            return true;
        }
    }
}