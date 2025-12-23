using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonFrameworks.Extensions;

public static class TagExtensions {
    public static bool HasAnyTag(this GameObject obj, List<string> tags) {
        return tags.Count == 0 || tags.TrueForAll(obj.CompareTag);
    }
        
    public static bool HasAnyTag(this GameObject obj, IEnumerable<string> tags) {
        return obj.HasAnyTag(tags.ToList());
    }
        
    public static bool HasAnyTag(this GameObject obj, params string[] tags) {
        return tags.Length == 0 || tags.All(obj.CompareTag);
    }
        
    public static bool HasAnyTag(this Component comp, List<string> tags) {
        return tags.Count == 0 || tags.TrueForAll(comp.CompareTag);
    }
        
    public static bool HasAnyTag(this Component comp, IEnumerable<string> tags) {
        return comp.HasAnyTag(tags.ToList());
    }
        
    public static bool HasAnyTag(this Component comp, params string[] tags) {
        return tags.Length == 0 || tags.All(comp.CompareTag);
    }

    public static bool HasNoneOfTags(this GameObject obj, List<string> tags) {
        return tags.Count == 0 || !tags.Exists(obj.CompareTag);
    }
        
    public static bool HasNoneOfTags(this GameObject obj, IEnumerable<string> tags) {
        return obj.HasNoneOfTags(tags.ToList());
    }
        
    public static bool HasNoneOfTags(this GameObject obj, params string[] tags) {
        return tags.Length == 0 || !tags.Any(obj.CompareTag);
    }
        
    public static bool HasNoneOfTags(this Component comp, List<string> tags) {
        return tags.Count == 0 || !tags.Exists(comp.CompareTag);
    }
        
    public static bool HasNoneOfTags(this Component comp, IEnumerable<string> tags) {
        return comp.HasNoneOfTags(tags.ToList());
    }
        
    public static bool HasNoneOfTags(this Component comp, params string[] tags) {
        return tags.Length == 0 || !tags.Any(comp.CompareTag);
    }
}