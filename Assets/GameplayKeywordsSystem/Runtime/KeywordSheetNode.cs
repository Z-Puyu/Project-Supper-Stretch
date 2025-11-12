using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayKeywordsSystem.Runtime {
    [Serializable]
    public sealed class KeywordSheetNode : IComparable<KeywordSheetNode> {
        [field: SerializeField, ReadOnly] internal string Name { get; private set; }
        [field: SerializeField, ReadOnly] internal string Path { get; private set; }

        [field: SerializeField, ReadOnly]
        internal List<KeywordSheetNode> Children { get; private set; } = new List<KeywordSheetNode>();
        
        internal bool IsLeaf => this.Children.Count == 0;
        
        internal KeywordSheetNode(string name, string fullName) {
            this.Name = name.ToLower();
        }

        private bool HasSameName(string name) {
            return string.Equals(this.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        internal bool Contains(string childName, out KeywordSheetNode node) {
            node = this.Children.Find(child => child.HasSameName(childName));
            return node is not null;
        }
        
        internal KeywordSheetNode FindOrAddChild(List<string> path) {
            string name = path[^1].ToLower();
            KeywordSheetNode node = this.Children.Find(child => child.Name == name);
            if (node is null) {
                node = new KeywordSheetNode(name, string.Join('.', path).ToLower());
                this.Children.Add(node);
            }
            
            this.Children.Sort();
            return node;
        }

        internal AdvancedDropdownList<string> ToDropdownList() {
            if (this.IsLeaf) {
                return new AdvancedDropdownList<string>(this.Name, this.Path);
            }
            
            List<AdvancedDropdownList<string>> children = this.Children.ConvertAll(child => child.ToDropdownList());
            children.Sort();
            return new AdvancedDropdownList<string>(this.Name, children);
        }

        public int CompareTo(KeywordSheetNode other) {
            if (object.ReferenceEquals(this, other)) {
                return 0;
            }

            return other is null ? 1 : string.Compare(this.Path, other.Path, StringComparison.Ordinal);
        }
    }
}
