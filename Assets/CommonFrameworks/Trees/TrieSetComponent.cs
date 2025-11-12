using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonFrameworks.Trees {
    public class TrieSetComponent<K, T> : MonoBehaviour, ITrie<K, T>, ISet<K> where K : IEnumerable<T> {
        private TrieSet<K, T> Trie { get; }

        public int Count => this.Trie.Count;
        public bool IsReadOnly => this.Trie.IsReadOnly;

        protected TrieSetComponent(TrieSet<K, T> trie) {
            this.Trie = trie;
        }

        public IEnumerator<K> GetEnumerator() {
            return this.Trie.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)this.Trie).GetEnumerator();
        }

        void ICollection<K>.Add(K item) {
            if (item is not null) {
                this.Trie.Add(item);
            }
        }

        public void ExceptWith(IEnumerable<K> other) {
            this.Trie.ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<K> other) {
            this.Trie.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<K> other) {
            return this.Trie.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<K> other) {
            return this.Trie.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<K> other) {
            return this.Trie.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<K> other) {
            return this.Trie.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<K> other) {
            return this.Trie.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<K> other) {
            return this.Trie.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<K> other) {
            this.Trie.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<K> other) {
            this.Trie.UnionWith(other);
        }

        public bool Add(K item) {
            return this.Trie.Add(item);
        }

        public void Clear() {
            this.Trie.Clear();
        }

        public bool Contains(K item) {
            return this.Trie.Contains(item);
        }

        public void CopyTo(K[] array, int arrayIndex) {
            this.Trie.CopyTo(array, arrayIndex);
        }

        public bool Remove(K item) {
            return this.Trie.Remove(item);
        }

        public bool ContainsPrefix(IEnumerable<T> prefix) {
            return this.Trie.ContainsPrefix(prefix);
        }

        public IEnumerable<K> PrefixSearch(IEnumerable<T> prefix) {
            return this.Trie.PrefixSearch(prefix);
        }

        public bool RemoveAllWithPrefix(IEnumerable<T> prefix) {
            return this.Trie.RemoveAllWithPrefix(prefix);
        }

        public bool Remove(IEnumerable<T> key) {
            return this.Trie.Remove(key);
        }
    }
}
