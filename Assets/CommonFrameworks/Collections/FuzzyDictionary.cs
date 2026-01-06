using System.Collections;
using System.Collections.Generic;

namespace CommonFrameworks.Collections {
    public sealed class FuzzyDictionary<K, T, V> : IDictionary<K, V>, IReadOnlyDictionary<K, V>
            where K : IEnumerable<T> {
        private TrieDictionary<K, T, V> Dictionary { get; } = new TrieDictionary<K, T, V>();

        public V this[K key] {
            get => this.TryGetValue(key, out V value) ? value : throw new KeyNotFoundException();
            set => this.Dictionary[key] = value;
        }

        IEnumerable<K> IReadOnlyDictionary<K, V>.Keys => this.Keys;
        IEnumerable<V> IReadOnlyDictionary<K, V>.Values => this.Values;
        public ICollection<K> Keys => this.Dictionary.Keys;
        public ICollection<V> Values => this.Dictionary.Values;
        public int Count => this.Dictionary.Count;
        public bool IsReadOnly => this.Dictionary.IsReadOnly;

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
            return this.Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void Add(KeyValuePair<K, V> item) {
            this.Dictionary.Add(item);
        }

        public void Clear() {
            this.Dictionary.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item) {
            return this.Dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
            this.Dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<K, V> item) {
            return this.Dictionary.Remove(item);
        }

        public void Add(K key, V value) {
            this.Dictionary.Add(key, value);
        }

        public bool ContainsKey(K key) {
            return this.Dictionary.ContainsKey(key);
        }

        bool IReadOnlyDictionary<K, V>.TryGetValue(K key, out V value) {
            return this.TryGetValue(key, out value);
        }

        public bool Remove(K key) {
            return this.Dictionary.Remove(key);
        }

        bool IReadOnlyDictionary<K, V>.ContainsKey(K key) {
            return this.ContainsKey(key);
        }

        public bool TryGetValue(K key, out V value) {
            if (this.Dictionary.TryGetValue(key, out value)) {
                return true;
            }

            if (!this.Dictionary.FindLongestPrefixKey(key, out KeyValuePair<K, V> prefix) &&
                !this.Dictionary.ContainsPrefixKey(key, out prefix)) {
                return false;
            }

            value = prefix.Value;
            return true;
        }
    }
}
