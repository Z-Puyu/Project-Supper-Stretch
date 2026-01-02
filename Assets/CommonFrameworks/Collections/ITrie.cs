using System.Collections.Generic;

namespace CommonFrameworks.Collections {
    /// <summary>
    /// A trie data structure.
    /// </summary>
    /// <typeparam name="U">The type of the items in the trie.</typeparam>
    /// <typeparam name="T">The type of the elements that form a key in the trie.</typeparam>
    public interface ITrie<U, in T> : ICollection<U> {
        /// <summary>
        /// Checks if the trie contains a prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns><c>true</c> if <paramref name="prefix"/> exists in the trie.</returns>
        public bool ContainsPrefix(IEnumerable<T> prefix);
        
        /// <summary>
        /// Collects all keys that start with the given prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>An <see cref="IEnumerable{K}"/> containing all keys
        /// starting with <paramref name="prefix"/>.</returns>
        public IEnumerable<U> PrefixSearch(IEnumerable<T> prefix);
        
        /// <summary>
        /// Removes all keys that start with the given prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns><c>true</c> if anything is removed,
        /// <c>false</c> if no key starts with <paramref name="prefix"/>.</returns>
        public bool RemoveAllWithPrefix(IEnumerable<T> prefix);
        
        /// <summary>
        /// Removes a key in the trie.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns><c>true</c> if <paramref name="key"/> is in the trie and successfully removed.</returns>
        public bool Remove(IEnumerable<T> key);
    }
}