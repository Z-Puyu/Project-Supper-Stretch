using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SaintsField.Samples.Scripts.SaintsEditor.Issues;

namespace CommonFrameworks.Trees {
    public sealed class TrieDictionary<K, T, V> : ITrie<KeyValuePair<K, V>, T>, IDictionary<K, V> where K : IEnumerable<T> {
        private sealed class Entry {
            internal IDictionary<T, Entry> Children { get; } = new Dictionary<T, Entry>();
            internal bool IsEndOfKey { get; set; }
            internal int Size { get; set; }
            internal V Value { get; set; }
        }
        
        private Entry Root { get; } = new Entry();
        private Func<IEnumerable<T>, K> KeyProducer { get; }
        private T Separator { get; }
        private bool HasSeparator { get; }
        
        private Lazy<IEnumerable<KeyValuePair<K, V>>> CachedEntries { get; set; }
        private Lazy<ICollection<K>> CachedKeys { get; set; }
        private Lazy<ICollection<V>> CachedValues { get; set; }
        
        private IEnumerable<KeyValuePair<K, V>> Entries => this.CachedEntries.Value;

        public TrieDictionary(Func<IEnumerable<T>, K> keyProducer) {
            this.KeyProducer = keyProducer;
            this.CachedEntries =
                    new Lazy<IEnumerable<KeyValuePair<K, V>>>(() => this.PrefixSearch(Enumerable.Empty<T>()));
            this.CachedKeys = new Lazy<ICollection<K>>(() => this.Entries.Select(entry => entry.Key).ToArray());
            this.CachedValues = new Lazy<ICollection<V>>(() => this.Entries.Select(entry => entry.Value).ToArray());
        }

        public TrieDictionary(Func<IEnumerable<T>, K> keyProducer, T separator) : this(keyProducer) {
            this.Separator = separator;
            this.HasSeparator = true;
        }

        private void InvalidateCachedCollections() {
            if (this.CachedEntries.IsValueCreated) {
                this.CachedEntries =
                        new Lazy<IEnumerable<KeyValuePair<K, V>>>(() => this.PrefixSearch(Enumerable.Empty<T>()));
            }

            if (this.CachedKeys.IsValueCreated) {
                this.CachedKeys = new Lazy<ICollection<K>>(() => this.Entries.Select(entry => entry.Key).ToArray());
            }

            if (this.CachedValues.IsValueCreated) {
                this.CachedValues = new Lazy<ICollection<V>>(() => this.Entries.Select(entry => entry.Value).ToArray());
            }
        }
        
        #region Dictionary Semantics
        
        public V this[K key] { 
            get => this.TryGetValue(key, out V value) ? value : throw new KeyNotFoundException();
            set {
                if (!key.Any()) {
                    throw new ArgumentException("The key in a trie cannot be empty!", nameof(key));    
                }
                
                List<Entry> path = this.Trace(key);
                path[^1].Value = value;
                if (!path[^1].IsEndOfKey) {
                    path[^1].IsEndOfKey = true;
                    foreach (Entry entry in path) {
                        entry.Size += 1;
                    }
                }
                
                this.InvalidateCachedCollections();
            }
        }
        
        public ICollection<K> Keys => this.CachedKeys.Value;
        public ICollection<V> Values => this.CachedValues.Value;
        public int Count => this.Root.Size;
        public bool IsReadOnly => false;
        
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
            return this.Entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        
        public void Add(KeyValuePair<K, V> item) {
            this.Add(item.Key, item.Value);
        }
        
        public void Clear() {
            this.Root.Children.Clear();
            this.Root.Size = 0;
            this.InvalidateCachedCollections();
        }
        
        public bool Contains(KeyValuePair<K, V> item) {
            return this.HasPath(item.Key, out List<Entry> path) && path[^1].IsEndOfKey &&  
                   path[^1].Value.Equals(item);
        }
        
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
            this.Entries.ToArray().CopyTo(array, arrayIndex);
        }
        
        public bool Remove(KeyValuePair<K, V> item) {
            return this.TryGetValue(item.Key, out V value) && value.Equals(item.Value) && this.Remove(item.Key);
        }
        
        public void Add(K key, V value) {
            if (!key.Any()) {
                throw new ArgumentException("The key in a trie cannot be empty!", nameof(key));    
            }
            
            List<Entry> path = this.Trace(key);
            if (path[^1].IsEndOfKey) {
                throw new ArgumentException($"An element with key {key} already exists!");
            }
            
            path[^1].IsEndOfKey = true;
            path[^1].Value = value;
            foreach (Entry entry in path) {
                entry.Size += 1;
            }
            
            this.InvalidateCachedCollections();
        }
        
        public bool ContainsKey(K key) {
            return this.HasPath(key, out List<Entry> path) && path[^1].IsEndOfKey;
        }
        
        public bool Remove(K key) {
            return this.Remove(key.AsEnumerable());
        }
        
        public bool TryGetValue(K key, out V value) {
            if (this.HasPath(key, out List<Entry> path) && path[^1].IsEndOfKey) {
                value = path[^1].Value;
                return true;
            }

            value = default;
            return false;
        }
        
        #endregion

        private List<Entry> Trace(IEnumerable<T> sequence) {
            List<Entry> path = new List<Entry>();
            Entry curr = this.Root;
            path.Add(curr);
            foreach (T element in sequence) {
                if (!curr.Children.TryGetValue(element, out Entry entry)) {
                    entry = new Entry();
                    curr.Children.Add(element, entry);
                }

                curr = entry;
                path.Add(curr);
            }

            return path;
        }
        
        private bool HasPath(IEnumerable<T> prefix, out List<Entry> path) {
            path = new List<Entry>();
            if (prefix is null) {
                return false;
            }
            
            path.Add(this.Root);
            foreach (T element in prefix) {
                if (!path[^1].Children.TryGetValue(element, out Entry entry)) {
                    return false;
                }
                
                path.Add(entry);
            }

            return !this.HasSeparator || path[^1].Children.ContainsKey(this.Separator);
        }
        
        public bool ContainsPrefix(IEnumerable<T> prefix) {
            return this.HasPath(prefix, out List<Entry> _);
        }
        
        public IEnumerable<KeyValuePair<K, V>> PrefixSearch(IEnumerable<T> prefix) {
            if (prefix is null) {
                return Enumerable.Empty<KeyValuePair<K, V>>();
            }

            T[] prefixArray = prefix.ToArray();
            if (!this.HasPath(prefixArray, out List<Entry> path)) {
                return Enumerable.Empty<KeyValuePair<K, V>>();
            }

            List<KeyValuePair<K, V>> entries = new List<KeyValuePair<K, V>>();
            Stack<(T element, Entry entry, int idx)> stack = new Stack<(T element, Entry entry, int idx)>();
            List<T> elements = new List<T>(prefixArray);
            foreach ((T element, Entry entry) in path[^1].Children) {
                stack.Push((element, entry, elements.Count));
            }

            while (stack.TryPop(out (T element, Entry entry, int idx) curr)) {
                if (elements.Count == curr.idx) {
                    elements.Add(curr.element);
                } else {
                    elements[curr.idx] = curr.element;
                }

                if (curr.entry.IsEndOfKey) {
                    K key = this.KeyProducer(curr.idx < elements.Count - 1 ? elements.Take(curr.idx + 1) : elements);
                    entries.Add(new KeyValuePair<K, V>(key, curr.entry.Value));
                } else {
                    foreach ((T element, Entry entry) in curr.entry.Children) {
                        stack.Push((element, entry, curr.idx + 1));
                    }
                }
            }

            return entries;
        }
        
        public bool RemoveAllWithPrefix(IEnumerable<T> prefix) {
            if (prefix is null) {
                return false;
            }
            
            T[] prefixArray = prefix.ToArray();
            if (!this.HasPath(prefixArray, out List<Entry> path)) {
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
            
            this.InvalidateCachedCollections();
            return true;
        }
        
        public bool Remove(IEnumerable<T> key) {
            if (key is null) {
                return false;   
            }
            
            T[] prefix = key.ToArray();
            if (!this.HasPath(prefix, out List<Entry> path) || path.Count == 0) {
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

            this.InvalidateCachedCollections();
            return true;   
        }
    }
}
