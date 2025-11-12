using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFrameworks.Trees {
    public class TrieDictionaryComponent<K, T, V> : MonoBehaviour, ITrie<KeyValuePair<K, V>, T>, IDictionary<K, V>
            where K : IEnumerable<T> {
        private TrieDictionary<K, T, V> Trie { get; }

        public V this[K key] { get => this.Trie[key]; set => this.Trie[key] = value; }
        public ICollection<K> Keys => this.Trie.Keys;
        public ICollection<V> Values => this.Trie.Values;
        public int Count => this.Trie.Count;
        public bool IsReadOnly => this.Trie.IsReadOnly;

        protected TrieDictionaryComponent(TrieDictionary<K, T, V> trie) {
            this.Trie = trie;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
            return this.Trie.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)this.Trie).GetEnumerator();
        }

        public void Add(KeyValuePair<K, V> item) {
            this.Trie.Add(item);
        }

        public void Clear() {
            this.Trie.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item) {
            return this.Trie.Contains(item);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
            this.Trie.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<K, V> item) {
            return this.Trie.Remove(item);
        }

        public bool ContainsPrefix(IEnumerable<T> prefix) {
            return this.Trie.ContainsPrefix(prefix);
        }

        public IEnumerable<KeyValuePair<K, V>> PrefixSearch(IEnumerable<T> prefix) {
            return this.Trie.PrefixSearch(prefix);
        }

        public bool RemoveAllWithPrefix(IEnumerable<T> prefix) {
            return this.Trie.RemoveAllWithPrefix(prefix);
        }

        public bool Remove(IEnumerable<T> key) {
            return this.Trie.Remove(key);
        }

        public void Add(K key, V value) {
            this.Trie.Add(key, value);
        }

        public bool ContainsKey(K key) {
            return this.Trie.ContainsKey(key);
        }

        public bool Remove(K key) {
            return this.Trie.Remove(key);
        }

        public bool TryGetValue(K key, out V value) {
            return this.Trie.TryGetValue(key, out value);
        }
    }
}
