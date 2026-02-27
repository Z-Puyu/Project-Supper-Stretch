using System.Collections.Generic;

namespace GameplayAbilities.Attributes {
    /// <summary>
    /// An interface for anything that can read attribute values from an owning game object.
    /// </summary>
    public interface IAttributeReader : IEnumerable<GameplayAttribute> {
        /// <summary>
        /// Get the current value of an attribute.
        /// </summary>
        /// <param name="key">The key of the attribute.</param>
        /// <returns>The current value of the attribute.</returns>
        public AttributeValue Query(GameplayAttributeType key);
        
        public double QueryMax(GameplayAttributeType key);
        
        public double QueryMin(GameplayAttributeType key);

        /// <summary>
        /// Checks if the owner has sufficient amount of attribute for a given key.
        /// </summary>
        /// <param name="threshold">The threshold to check against.</param>
        /// <param name="key">The key of the attribute.</param>
        /// <returns><c>true</c> if the owner has sufficient amount of attribute for the given key.</returns>
        public bool HasAtLeast(double threshold, GameplayAttributeType key);
        
        /// <summary>
        /// Checks if the owner has at most the specified amount of attribute for a given key.
        /// </summary>
        /// <param name="cap">The maximum allowed amount of attribute for the given key.</param>
        /// <param name="key">The key of the attribute.</param>
        /// <returns><c>true</c> if the owner has at most the specified amount of attribute for the given key.</returns>
        public bool HasAtMost(double cap, GameplayAttributeType key);
    }
}