using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayKeywordsSystem.Runtime {
    [Serializable]
    internal sealed class KeywordSheetNode : IComparable<KeywordSheetNode> {
        [field: SerializeField] internal string Name { get; private set; } = string.Empty;
        [field: SerializeField, ReadOnly] internal string Path { get; set; } = string.Empty;

        [field: SerializeField, DefaultExpand, FieldDefaultExpand]
        internal List<KeywordSheetNode> Children { get; private set; } = new List<KeywordSheetNode>();

        internal bool IsLeaf => this.Children.Count == 0;

        private bool HasSameName(string name) {
            return string.Equals(this.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        internal bool Contains(string childName, out KeywordSheetNode node) {
            node = this.Children.Find(child => child.HasSameName(childName));
            return node is not null;
        }

        internal (AdvancedDropdownList<string>?, AdvancedDropdownList<string>) Collapse(bool all = false) {
            if (this.IsLeaf) {
                return (null, new AdvancedDropdownList<string>(this.Name, this.Path));
            }

            List<AdvancedDropdownList<string>> list = new List<AdvancedDropdownList<string>>();
            foreach (KeywordSheetNode child in this.Children) {
                (AdvancedDropdownList<string>? self, AdvancedDropdownList<string> children) = child.Collapse(all);
                if (all && self is not null) {
                    list.Add(self);
                }

                list.Add(children);
            }

            list.Sort((a, b) => string.Compare(a.displayName, b.displayName, StringComparison.OrdinalIgnoreCase));
            return (new AdvancedDropdownList<string>(this.Name, this.Path),
                new AdvancedDropdownList<string>(this.Name, list));
        }

        public int CompareTo(KeywordSheetNode? other) {
            if (object.ReferenceEquals(this, other)) {
                return 0;
            }

            return other is null ? 1 : string.Compare(this.Path, other.Path, StringComparison.Ordinal);
        }
    }
}