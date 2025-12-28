using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilitiesSystem.Runtime.Attributes.Processors;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes;

[CreateAssetMenu(fileName = "New Attribute Type", menuName = "Gameplay Abilities/Attribute Type")]
public class AttributeType : ScriptableObject, IComparable<AttributeType>, IEquatable<AttributeType> {
    [field: SerializeField, ReadOnly] public string Id { get; private set; } = string.Empty;
    [field: SerializeField, ReadOnly] private AttributeType? Parent { get; set; }
    [field: SerializeField] private string Name { get; set; } = string.Empty;
    [SerializeField] private string displayName = string.Empty;

    [field: SerializeReference, ReferencePicker, ShowIf(nameof(this.IsLeaf))]
    public List<AttributeProcessor> Processors { get; private set; } = new List<AttributeProcessor>();
        
    [field: SerializeField]
    public List<AttributeType> SubTypes { get; private set; } = new List<AttributeType>();
        
    public string DisplayName => string.IsNullOrWhiteSpace(this.displayName) ? this.Name : this.displayName;
    public bool IsLeaf => this.SubTypes.Count == 0;
    public bool IsRoot => !this.Parent;

    public bool Includes(string attribute) {
        return this.Id == attribute || this.SubTypes.Any(type => type.Includes(attribute));
    }

    private void OnValidate() {
        if (this.IsLeaf) {
            this.Rename();
        }
    }

#if UNITY_EDITOR
    private void Rename() {
        LinkedList<string> names = new LinkedList<string>();
        AttributeType? curr = this;
        while (curr) {
            names.AddFirst(curr.Name);
            curr = curr.Parent;
        }
            
        this.Id = string.Join(".", names);
        foreach (AttributeType def in this.SubTypes) {
            if (!def) {
                continue;
            }
                
            def.Parent = this;
            def.Rename();
        }
    }
#endif

    public int CompareTo(AttributeType other) {
        return other ? string.CompareOrdinal(this.Id, other.Id) : 1;
    }
        
    public bool Equals(AttributeType other) {
        return this.CompareTo(other) == 0;
    }

    internal AdvancedDropdownList<string> ToAdvancedDropdownList() {
        if (this.IsLeaf) {
            return new AdvancedDropdownList<string>(this.DisplayName, this.Id);
        }

        List<AdvancedDropdownList<string>> children =
                this.SubTypes.ConvertAll(child => child.ToAdvancedDropdownList());
        children.Sort((a, b) => string.CompareOrdinal(a.displayName, b.displayName));
        return new AdvancedDropdownList<string>(this.DisplayName, children);
    }

    public static IEnumerable<AttributeType> GetAllLeaves() {
        return Resources.LoadAll<AttributeType>("").Where(a => a.IsLeaf).OrderBy(a => a.Id);
    }
}