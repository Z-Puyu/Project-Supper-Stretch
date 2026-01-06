using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
        public bool ContainsPrefix<P>(P prefix) where P : IEnumerable<T>;

        /// <summary>
        /// Checks if the trie contains a key that is a prefix of the given sequence.
        /// </summary>
        /// <param name="sequence">The sequence to check for a prefix key.</param>
        /// <param name="key">The shortest key that is a prefix of <paramref name="sequence"/>.</param>
        /// <returns><c>true</c> if a key that is a prefix for <paramref name="sequence"/> exists in the trie.</returns>
        public bool ContainsPrefixKey<S>(S sequence, [NotNullWhen(true)] out U? key) where S : IEnumerable<T>;

        /// <summary>
        /// Finds the longest key in the trie that is a prefix of the given sequence.
        /// </summary>
        /// <param name="sequence">The sequence to find the longest prefix key for.</param>
        /// <param name="key">The longest key that is a prefix of <paramref name="sequence"/>.</param>
        /// <returns><c>true</c> if a key that is a prefix for <paramref name="sequence"/> exists in the trie.</returns>
        public bool FindLongestPrefixKey<S>(S sequence, [NotNullWhen(true)] out U? key) where S : IEnumerable<T>;

        /// <summary>
        /// Collects all keys that start with the given prefix using a breath-first search.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>An <see cref="IList{U}"/> containing all keys starting with <paramref name="prefix"/>.</returns>
        public IList<U> BreathFirstPrefixSearch<P>(P prefix) where P : IEnumerable<T>;

        /// <summary>
        /// Collects all keys that start with the given prefix using a depth-first search.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>An <see cref="IList{U}"/> containing all keys starting with <paramref name="prefix"/>.</returns>
        public IList<U> DepthFirstPrefixSearch<P>(P prefix) where P : IEnumerable<T>;

        /// <summary>
        /// Removes all keys that start with the given prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns><c>true</c> if anything is removed,
        /// <c>false</c> if no key starts with <paramref name="prefix"/>.</returns>
        public bool RemoveAllWithPrefix<P>(P prefix) where P : IEnumerable<T>;

        /// <summary>
        /// Removes all keys that start with the given prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="removed">The removed keys.</param>
        /// <returns><c>true</c> if anything is removed,
        /// <c>false</c> if no key starts with <paramref name="prefix"/>></returns>
        public bool RemoveAllWithPrefix<P>(P prefix, out IEnumerable<U> removed) where P : IEnumerable<T>;
    }
}