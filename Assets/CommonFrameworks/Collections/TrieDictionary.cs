using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommonFrameworks.Collections {
    public sealed class TrieDictionary<K, T, V> : ITrie<KeyValuePair<K, V>, T>, IDictionary<K, V> where K : IEnumerable<T> {
        private sealed class Entry {
            internal IDictionary<T, Entry> Children { get; } = new Dictionary<T, Entry>();
            internal bool IsEndOfKey { get; set; }
            internal int Size { get; set; }
            internal K Key { get; set; } = default!;
            internal V Value { get; set; } = default!;
        }
        
        private Entry Root { get; } = new Entry();
        private T Separator { get; } = default!;
        private bool HasSeparator { get; }
        
        private Lazy<IEnumerable<KeyValuePair<K, V>>> CachedEntries { get; set; }
        private Lazy<ICollection<K>> CachedKeys { get; set; }
        private Lazy<ICollection<V>> CachedValues { get; set; }
        
        private IEnumerable<KeyValuePair<K, V>> Entries => this.CachedEntries.Value;

        public TrieDictionary() {
            this.CachedEntries =
                    new Lazy<IEnumerable<KeyValuePair<K, V>>>(() => this.BreathFirstPrefixSearch(Enumerable.Empty<T>()));
            this.CachedKeys = new Lazy<ICollection<K>>(() => this.Entries.Select(entry => entry.Key).ToArray());
            this.CachedValues = new Lazy<ICollection<V>>(() => this.Entries.Select(entry => entry.Value).ToArray());
        }

        public TrieDictionary(T separator) : this() {
            this.Separator = separator;
            this.HasSeparator = true;
        }

        private void InvalidateCachedCollections() {
            if (this.CachedEntries.IsValueCreated) {
                this.CachedEntries =
                        new Lazy<IEnumerable<KeyValuePair<K, V>>>(() => this.BreathFirstPrefixSearch(Enumerable.Empty<T>()));
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
            get => this.TryGetValue(key, out V value) && value is not null ? value : throw new KeyNotFoundException();
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
                   EqualityComparer<V>.Default.Equals(path[^1].Value, item.Value);
        }
        
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
            this.Entries.ToArray().CopyTo(array, arrayIndex);
        }
        
        public bool Remove(KeyValuePair<K, V> item) {
            return this.TryGetValue(item.Key, out V value) && 
                   EqualityComparer<V>.Default.Equals(value, item.Value) &&
                   this.Remove(item.Key);
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
            path[^1].Key = key;
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

            value = default!;
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
            path = new List<Entry> { this.Root };
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
        
        public IList<KeyValuePair<K, V>> BreathFirstPrefixSearch(IEnumerable<T> prefix) {
            T[] prefixArray = prefix.ToArray();
            if (!this.HasPath(prefixArray, out List<Entry> path)) {
                return new List<KeyValuePair<K, V>>();
            }

            List<KeyValuePair<K, V>> entries = new List<KeyValuePair<K, V>>();
            Queue<Entry> queue = new Queue<Entry>();
            queue.Enqueue(path[^1]);
            while (queue.TryDequeue(out Entry curr)) {
                if (curr.IsEndOfKey) {
                    entries.Add(new KeyValuePair<K, V>(curr.Key, curr.Value));
                } else {
                    foreach (Entry entry in curr.Children.Values) {
                        queue.Enqueue(entry);
                    }
                }
            }

            return entries;
        }

        public IList<KeyValuePair<K, V>> DepthFirstPrefixSearch(IEnumerable<T> prefix) {
            T[] prefixArray = prefix.ToArray();
            if (!this.HasPath(prefixArray, out List<Entry> path)) {
                return new List<KeyValuePair<K, V>>();
            }

            List<KeyValuePair<K, V>> entries = new List<KeyValuePair<K, V>>();
            Stack<Entry> stack = new Stack<Entry>();
            stack.Push(path[^1]);
            while (stack.TryPop(out Entry curr)) {
                if (curr.IsEndOfKey) {
                    entries.Add(new KeyValuePair<K, V>(curr.Key, curr.Value));
                } else {
                    foreach (Entry entry in curr.Children.Values) {
                        stack.Push(entry);
                    }
                }
            }

            return entries;
        }
        
        public bool RemoveAllWithPrefix(IEnumerable<T> prefix) {
            T[] prefixArray = prefix.ToArray();
            if (!this.HasPath(prefixArray, out List<Entry> path)) {
                return false;
            }
            
            path[^1].Children.Clear();
            path[^1].IsEndOfKey = false;
            int size = path[^1].Size;
            for (int i = 1; i < prefixArray.Length; i += 1) {
                path[i].Size -= size;
                if (path[i].Size > 0) {
                    continue;
                }
                
                path[i - 1].Children.Remove(prefixArray[i]);
                break;
            }
            
            this.Root.Size -= size;
            this.InvalidateCachedCollections();
            return true;
        }

        public bool RemoveAllWithPrefix(IEnumerable<T> prefix, out IEnumerable<KeyValuePair<K, V>> removed) {
            T[] prefixArray = prefix.ToArray();
            IList<KeyValuePair<K, V>> removedEntries = this.BreathFirstPrefixSearch(prefixArray);
            removed = removedEntries;
            if (removedEntries.Count == 0) {
                return false;
            }

            if (this.HasPath(prefixArray, out List<Entry> path)) {
                for (int i = 1; i < prefixArray.Length; i += 1) {
                    path[i].Size -= removedEntries.Count;
                    if (path[i].Size > 0) {
                        continue;
                    }
                    
                    path[i - 1].Children.Remove(prefixArray[i]);
                    break;
                }
            }
            
            this.Root.Size -= removedEntries.Count;
            this.InvalidateCachedCollections();
            return true;
        }

        public bool Remove(IEnumerable<T> key) {
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