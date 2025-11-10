using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommonFrameworks.Trees {
    public sealed class TrieSet<K, T> : ITrie<K, T> where K : IEnumerable<T> {
        private sealed class Node {
            internal IDictionary<T, Node> Children { get; } = new Dictionary<T, Node>();
            internal bool IsEndOfKey { get; set; }
            internal int Size { get; set; }
        }

        private Node Root { get; } = new Node();
        private Func<IEnumerable<T>, K> KeyProducer { get; }
        private T Separator { get; }
        private bool HasSeparator { get; }

        public TrieSet(Func<IEnumerable<T>, K> keyProducer) {
            this.KeyProducer = keyProducer;
        }

        public TrieSet(Func<IEnumerable<T>, K> keyProducer, T separator) : this(keyProducer) {
            this.Separator = separator;
            this.HasSeparator = true;
        }

        #region Collection Semantics

        public int Count => this.Root.Size;
        public bool IsReadOnly => false;

        public IEnumerator<K> GetEnumerator() {
            return this.PrefixSearch(Enumerable.Empty<T>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void Add(K item) {
            if (item is null) {
                return;
            }

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
                return;
            }

            curr.IsEndOfKey = true;
            foreach (Node node in path) {
                node.Size += 1;
            }
        }

        public void Clear() {
            this.Root.Children.Clear();
            this.Root.Size = 0;
        }

        public bool Contains(K item) {
            return item is not null && this.HasPath(item, out List<Node> path) && path[^1].IsEndOfKey;
        }

        public void CopyTo(K[] array, int arrayIndex) {
            this.PrefixSearch(Enumerable.Empty<T>()).ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(K item) {
            return item is not null && this.Remove(item.AsEnumerable());       
        }

        #endregion

        private bool HasPath(IEnumerable<T> prefix, out List<Node> path) {
            path = new List<Node>();
            if (prefix is null) {
                return false;
            }
            
            path.Add(this.Root);
            foreach (T element in prefix) {
                if (!path[^1].Children.TryGetValue(element, out Node node)) {
                    return false;
                }
                
                path.Add(node);
            }

            return !this.HasSeparator || path[^1].Children.ContainsKey(this.Separator);
        }

        public bool ContainsPrefix(IEnumerable<T> prefix) {
            return this.HasPath(prefix, out List<Node> _);
        }

        public IEnumerable<K> PrefixSearch(IEnumerable<T> prefix) {
            if (prefix is null) {
                return Enumerable.Empty<K>();
            }

            T[] prefixArray = prefix.ToArray();
            if (!this.HasPath(prefixArray, out List<Node> path)) {
                return Enumerable.Empty<K>();
            }

            List<K> keys = new List<K>();
            Stack<(T element, Node node, int idx)> stack = new Stack<(T element, Node node, int idx)>();
            List<T> elements = new List<T>(prefixArray);
            foreach ((T element, Node node) in path[^1].Children) {
                stack.Push((element, node, elements.Count));
            }

            while (stack.TryPop(out (T element, Node node, int idx) curr)) {
                if (elements.Count == curr.idx) {
                    elements.Add(curr.element);
                } else {
                    elements[curr.idx] = curr.element;
                }

                if (curr.node.IsEndOfKey) {
                    keys.Add(this.KeyProducer(curr.idx < elements.Count - 1 ? elements.Take(curr.idx + 1) : elements));
                } else {
                    foreach ((T element, Node node) in curr.node.Children) {
                        stack.Push((element, node, curr.idx + 1));
                    }
                }
            }

            return keys;
        }

        public bool RemoveAllWithPrefix(IEnumerable<T> prefix) {
            if (prefix is null) {
                return false;
            }
            
            T[] prefixArray = prefix.ToArray();
            if (!this.HasPath(prefixArray, out List<Node> path)) {
                return false;
            }
            
            path[^1].Children.Clear();
            path[^1].IsEndOfKey = false;
            int size = path[^1].Size;
            int idx = 1;
            this.Root.Size -= size;
            foreach (T element in prefixArray) {
                path[idx].Size -= size;
                if (path[idx].Size == 0) {
                    path[idx - 1].Children.Remove(element);
                    break;
                }
                
                idx += 1;
            }
            
            return true;
        }

        public bool Remove(IEnumerable<T> key) {
            if (key is null) {
                return false;   
            }
            
            T[] prefix = key.ToArray();
            if (!this.HasPath(prefix, out List<Node> path) || path.Count == 0) {
                return false;
            }

            if (!path[^1].IsEndOfKey) {
                return false;
            }

            path[^1].IsEndOfKey = false;
            this.Root.Size -= 1;
            int idx = 1;
            foreach (T element in prefix) {
                path[idx].Size -= 1;
                if (path[idx].Size == 0) {
                    path[idx - 1].Children.Remove(element);
                    break;
                }
                
                idx += 1;
            }

            return true;     
        }
    }
}
