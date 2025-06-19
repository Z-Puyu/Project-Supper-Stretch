using System;
using System.Linq;
using Editor;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.Definitions;

[Serializable]
public struct AttributeKey : IEquatable<AttributeKey> {
#if UNITY_EDITOR
    public static AdvancedDropdownList<AttributeKey> AllDefinitions => 
            ObjectCache<AttributeDefinition>.Instance.Objects.FetchAll();
#endif
    
    [field: SerializeField] public string FullName { get; set; }
    [field: SerializeField] public string Name { get; set; }
    public string Root => this.FullName.Split('.').First();

    private AttributeKey(string fullName, string name) {
        this.FullName = fullName;       
        this.Name = name;       
    }

    public static AttributeKey OfPath(params string[] path) {
        return new AttributeKey(string.Join('.', path), path.Last());
    }

    public static AttributeKey OfPath(string path) {
        int lastSplit = path.LastIndexOf('.');
        if (lastSplit == 0 || lastSplit == path.Length - 1) {
            throw new ArgumentException($"Invalid path {path}. Split must not be at the two ends of the path.");
        }
        
        return new AttributeKey(path, path[(lastSplit + 1)..]);
    }

    public override string ToString() {
        return this.FullName;
    }

    public override bool Equals(object? obj) {
        return obj is AttributeKey key && key == this;
    }

    public bool Equals(AttributeKey other) {
        return this.FullName == other.FullName;
    }

    public override int GetHashCode() {
        return HashCode.Combine(this.FullName);
    }

    public static implicit operator AttributeKey(string path) {
        return AttributeKey.OfPath(path);       
    }

    public static bool operator ==(AttributeKey left, AttributeKey right) {
        return left.FullName == right.FullName;
    }

    public static bool operator !=(AttributeKey left, AttributeKey right) {
        return !(left == right);
    }

    public static bool operator ==(AttributeKey left, string right) {
        return left.FullName == right;
    }

    public static bool operator !=(AttributeKey left, string right) {
        return !(left == right);
    }
}
