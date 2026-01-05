using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommonFrameworks.Collections {
    public sealed class Multimap<V, K, C> : ILookup<V, K>, IReadOnlyDictionary<V, IEnumerable<K>>, IReadOnlyDictionary<V, ICollection<K>> where C : class, ICollection<K>, new() {
        private Dictionary<V, C> Map { get; } = new Dictionary<V, C>();
        
        public int Count { get; private set; }

        public IEnumerable<K> this[V key] => this.Map.TryGetValue(key, out C bucket) ? bucket : Enumerable.Empty<K>();
        
        ICollection<K> IReadOnlyDictionary<V, ICollection<K>>.this[V key] =>
                this.Map.TryGetValue(key, out C bucket) ? bucket : new K[] { };
        
        public IEnumerable<V> Keys => this.Map.Keys;
        IEnumerable<IEnumerable<K>> IReadOnlyDictionary<V, IEnumerable<K>>.Values => this.Map.Values;
        public IEnumerable<ICollection<K>> Values => this.Map.Values;

        IEnumerator<KeyValuePair<V, ICollection<K>>> IEnumerable<KeyValuePair<V, ICollection<K>>>.GetEnumerator() {
            foreach ((V key, ICollection<K> value) in this.Map) {
                yield return new KeyValuePair<V, ICollection<K>>(key, value);
            }
        }

        IEnumerator<KeyValuePair<V, IEnumerable<K>>> IEnumerable<KeyValuePair<V, IEnumerable<K>>>.GetEnumerator() {
            foreach ((V key, ICollection<K> value) in this.Map) {
                yield return new KeyValuePair<V, IEnumerable<K>>(key, value);
            }
        }

        public IEnumerator<IGrouping<V, K>> GetEnumerator() {
            foreach ((V key, ICollection<K> value) in this.Map) {
                yield return new Grouping(key, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public bool Contains(V key) {
            return this.Map.ContainsKey(key);
        }
        
        public bool ContainsKey(V key) {
            return this.Contains(key);
        }
        
        public bool TryGetValue(V key, out ICollection<K> value) {
            if (this.Map.TryGetValue(key, out C collection)) {
                value = collection;
                return true;
            }
            
            value = null!;
            return false;
        }
        
        public bool TryGetValue(V key, out IEnumerable<K> value) {
            if (this.Map.TryGetValue(key, out C collection)) {
                value = collection;
                return true;
            }
            
            value = null!;
            return false;
        }

        public void Add(V key, K value) {
            if (this.Map.TryGetValue(key, out C bucket)) {
                bucket.Add(value);
            } else {
                bucket = new C { value };
                this.Map.Add(key, bucket);
            }
            
            this.Count += 1;
        }
        
        public void Add(V key, IEnumerable<K> values) {
            if (!this.Map.TryGetValue(key, out C bucket)) {
                bucket = new C();
                this.Map.Add(key, bucket);
            } 
            
            foreach (K value in values) {
                bucket.Add(value);
                this.Count += 1;
            }
        }
        
        public void Clear() {
            this.Map.Clear();
            this.Count = 0;
        }
        
        public bool Clear(V key) {
            return this.Map.Remove(key);
        }
        
        public bool Remove(V key, K value) {
            if (!this.Map.TryGetValue(key, out C bucket) || !bucket.Remove(value)) {
                return false;
            }

            this.Count -= 1;
            return true;
        }

        public bool Remove(V key, Func<K, bool> predicate) {
            if (!this.Map.TryGetValue(key, out C bucket)) {
                return false;
            }
            
            IEnumerable<K> toRemove = bucket.Where(predicate);
            bool removed = false;
            foreach (K value in toRemove) {
                removed = true;
                bucket.Remove(value);
                this.Count -= 1;
            }

            return removed;
        }
        
        public bool Remove(Func<K, bool> predicate) {
            bool removed = false;
            foreach ((V key, C bucket) in this.Map) {
                IEnumerable<K> toRemove = bucket.Where(predicate);
                foreach (K value in toRemove) {
                    removed = true;
                    bucket.Remove(value);
                    this.Count -= 1;
                }
            }
            
            return removed;
        }

        private sealed class Grouping : IGrouping<V, K> {
            public V Key { get; }
            private IEnumerable<K> Values { get; }

            internal Grouping(V key, IEnumerable<K> values) {
                this.Key = key;
                this.Values = values;
            }
            
            public IEnumerator<K> GetEnumerator() {
                return this.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return this.GetEnumerator();
            }
        }
    }
}
