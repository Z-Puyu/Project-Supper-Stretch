using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommonFrameworks.Maths;
using GameplayAbilities.Attributes.Evaluation;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Attributes {
    [Serializable]
    internal sealed class AttributeType : IComparable<AttributeType>, IEquatable<AttributeType> {
        [field: SerializeField, ReadOnly] public string Id { get; private set; } = string.Empty;
        [field: SerializeField] private string Name { get; set; } = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        
        [field: SerializeReference, ReferencePicker, ShowIf(nameof(this.IsLeaf))]
        internal IEvaluable<IAttributeReader>? MinValue { get; private set; } = new Constant();
        
        [field: SerializeReference, ReferencePicker, ShowIf(nameof(this.IsLeaf))]
        internal IEvaluable<IAttributeReader>? MaxValue { get; private set; }
        
        [field: SerializeReference, ReferencePicker, ShowIf(nameof(this.IsLeaf))]
        internal AttributeApproximator? ApproximatorOverride { get; private set; }
        
        [field: SerializeField, ShowIf(nameof(this.IsLeaf))]
        internal AttributeCalculator? Derivation { get; private set; }
        
        [field: SerializeField] public List<AttributeType> SubTypes { get; private set; } = new List<AttributeType>();
        
        public string DisplayName => string.IsNullOrWhiteSpace(this.displayName) ? this.Name : this.displayName;
        private bool IsLeaf => this.SubTypes.Count == 0;
        
        public int CompareTo(AttributeType? other) {
            if (other is null) {
                return 1;
            }
            
            IEnumerable<object> dependencies = Enumerable.Empty<object>();
            if (this.MinValue is not null) {
                dependencies = dependencies.Concat(this.MinValue.DependentParameters);
            }
            
            if (this.MaxValue is not null) {
                dependencies = dependencies.Concat(this.MaxValue.DependentParameters);
            }
            
            if (this.Derivation is not null) {
                dependencies = dependencies.Concat(this.Derivation.DependentParameters);
            }
            
            HashSet<object> set = dependencies.ToHashSet();
            return dependsOn(other) ? 1 : string.CompareOrdinal(this.Id, other.Id);

            bool dependsOn(AttributeType type) {
                return set.Contains(other) ||
                       set.Any(item => item is string id && id == type.Id) ||
                       set.Any(item => item is AttributeKey key && key == type.Id);
            }
        }
        
        public bool Equals(AttributeType other) {
            return this.CompareTo(other) == 0;
        }

        internal bool HasChild(string id, [NotNullWhen(true)] out AttributeType? child) {
            if (id == this.Id) {
                child = this;
                return true;
            }
            
            if (id.StartsWith(this.Id)) {
                foreach (AttributeType type in this.SubTypes) {
                    if (type.HasChild(id, out child)) {
                        return true;
                    }
                }    
            }
            
            child = null;
            return false;
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
        
#if UNITY_EDITOR
        internal void Validate(string parent = "") {
            this.Id = string.IsNullOrEmpty(parent) ? this.Name : $"{parent}/{this.Name}";
            foreach (AttributeType def in this.SubTypes) {
                def?.Validate(this.Id);
            }
        }
#endif
    }
}