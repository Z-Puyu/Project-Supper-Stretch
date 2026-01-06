using System.Collections.Generic;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    /// <summary>
    /// An interface for anything that can read attribute values from an owning game object.
    /// </summary>
    public interface IAttributeReader : IEnumerable<Attribute> {
        /// <summary>
        /// Get the current value of an attribute.
        /// </summary>
        /// <param name="key">The key of the attribute.</param>
        /// <returns>The current value of the attribute.</returns>
        public double Query(AttributeKey key);

        /// <summary>
        /// Checks if the owner has sufficient amount of attribute for a given key.
        /// </summary>
        /// <param name="threshold">The threshold to check against.</param>
        /// <param name="key">The key of the attribute.</param>
        /// <returns><c>true</c> if the owner has sufficient amount of attribute for the given key.</returns>
        public bool HasAtLeast(double threshold, AttributeKey key);
    }
}