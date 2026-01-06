using System;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Processors;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    [Serializable]
    public sealed class AttributeType : IComparable<AttributeType>, IEquatable<AttributeType> {
        [field: SerializeField, ReadOnly] public string Id { get; private set; } = string.Empty;
        [field: SerializeField] private string Name { get; set; } = string.Empty;
        [SerializeField] private string displayName = string.Empty;

        [field: SerializeReference, ReferencePicker, ShowIf(nameof(this.IsLeaf))]
        public List<IProcessor<Attribute>> Processors { get; private set; } = new List<IProcessor<Attribute>>();
        
        [field: SerializeField]
        public List<AttributeType> SubTypes { get; private set; } = new List<AttributeType>();
        
        public string DisplayName => string.IsNullOrWhiteSpace(this.displayName) ? this.Name : this.displayName;
        private bool IsLeaf => this.SubTypes.Count == 0;
        
#if UNITY_EDITOR
        internal void Validate(string parent = "") {
            this.Id = string.IsNullOrEmpty(parent) ? this.Name : $"{parent}/{this.Name}";
            foreach (AttributeType def in this.SubTypes) {
                def?.Validate(this.Id);
            }
        }
#endif

        public int CompareTo(AttributeType? other) {
            return other is not null ? string.CompareOrdinal(this.Id, other.Id) : 1;
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
        
        internal AdvancedDropdownList<AttributeType> ToObjectAdvancedDropdownList() {
            if (this.IsLeaf) {
                return new AdvancedDropdownList<AttributeType>(this.DisplayName, this);
            }

            List<AdvancedDropdownList<AttributeType>> children =
                    this.SubTypes.ConvertAll(child => child.ToObjectAdvancedDropdownList());
            children.Sort((a, b) => string.CompareOrdinal(a.displayName, b.displayName));
            return new AdvancedDropdownList<AttributeType>(this.DisplayName, children);
        }
    }
}