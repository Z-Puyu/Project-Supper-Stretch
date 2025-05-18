using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes;

public class AttributeManagementSystem : MonoBehaviour {
    private Dictionary<string, AttributeSet> Sets { get; set; } = [];
    
    private void Awake() {
        foreach (AttributeSet set in this.GetComponentsInChildren<AttributeSet>(true)) {
            if (!this.Sets.TryAdd(set.Tag, set)) {
                throw new Exception($"Duplicate attribute sets with tag {set.Tag} in {this.gameObject}!");
            }
        }
    }

    /// <summary>
    /// Checks if there is an attribute set with the given tag in the system.
    /// </summary>
    /// <param name="label">The tag of the attribute set.</param>
    /// <param name="set">The attribute set if found.</param>
    /// <returns>True if the attribute set was found, false otherwise.</returns>
    public bool FoundAttributeSet(string label, out AttributeSet set) {
        return this.Sets.TryGetValue(label, out set);
    }
    
    /// <summary>
    /// Finds all attribute sets that use the given enum type as the attribute keys.
    /// </summary>
    /// <typeparam name="K">The enum type.</typeparam>
    /// <returns>All attribute sets that use the given enum type.</returns>
    public IEnumerable<AttributeSet> AllSetsUsing<K>() where K : Enum {
        Type category = typeof(K);
        return this.Sets.Values.Where(set => set.AttributeCategory == category);
    }
    
    /// <summary>
    /// Searches for an attribute in the system.
    /// </summary>
    /// <param name="key">The attribute to search for.</param>
    /// <typeparam name="K">The enum type of the attribute.</typeparam>
    /// <returns>The first attribute with the given key found in the system.
    /// If the system does not contain this attribute, return a zero attribute.</returns>
    public Attribute Query<K>(K key) where K : Enum {
        foreach (AttributeSet set in this.Sets.Values) {
            if (set.FoundAttribute(key, out Attribute attribute)) {
                return attribute;
            }
        }

        return Attribute.Zero(key);
    }

    /// <summary>
    /// Searches for an attribute in the system.
    /// </summary>
    /// <param name="label">The tag of the attribute set to search in.</param>
    /// <param name="key">The attribute to search for.</param>
    /// <typeparam name="K">The enum type of the attribute.</typeparam>
    /// <returns>The attribute with the given key found in the attribute set with the given tag.
    /// If no such attribute is found, return a zero attribute.</returns>
    public Attribute Query<K>(string label, K key) where K : Enum {
        return this.FoundAttributeSet(label, out AttributeSet set) ? set[key] : Attribute.Zero(key);
    }
}
