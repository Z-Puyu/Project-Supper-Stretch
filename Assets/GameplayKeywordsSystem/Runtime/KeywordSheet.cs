using System;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Iterators;
using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;

namespace GameplayKeywordsSystem.Runtime {
    [CreateAssetMenu(fileName = "New Keyword Sheet", menuName = "Gameplay Keywords/Keyword Sheet")]
    public sealed class KeywordSheet : ScriptableObject, ITraversable<KeywordSheetNode> {
        [field: SerializeField, DefaultExpand, FieldDefaultExpand]
        private List<KeywordSheetNode> Keywords { get; set; } = new List<KeywordSheetNode>();

        KeywordSheetNode? ITraversable<KeywordSheetNode>.Start => this.Keywords.FirstOrDefault();

        private void OnDestroy() {
            Database<KeywordSheet>.Reload();
        }

        bool ITraversable<KeywordSheetNode>.HasOutNeighbours(
            KeywordSheetNode vertex, out IEnumerable<KeywordSheetNode> children
        ) {
            children = vertex.Children;
            return vertex.Children.Count > 0;
        }

        private bool Contains(string keyword) {
            string[] parts = keyword.Trim().ToLower().Split('/', StringSplitOptions.RemoveEmptyEntries);
            KeywordSheetNode root = this.Keywords.Find(node => node.Name == parts[0]);
            if (root is null) {
                return false;
            }

            KeywordSheetNode curr = root;
            for (int i = 1; i < parts.Length; i += 1) {
                if (!curr.Contains(parts[i], out curr)) {
                    return false;
                }
            }

            return true;
        }

        internal IEnumerable<AdvancedDropdownList<string>> ToAdvancedDropdownLists(bool includeInternalNodes = false) {
            List<AdvancedDropdownList<string>> list = new List<AdvancedDropdownList<string>>();
            foreach (KeywordSheetNode child in this.Keywords) {
                (AdvancedDropdownList<string>? self, AdvancedDropdownList<string> children) =
                        child.ToAdvancedDropdownList(includeInternalNodes);
                if (includeInternalNodes && self is not null) {
                    list.Add(self);
                }
                
                list.Add(children);
            }

            list.Sort((a, b) => string.Compare(a.displayName, b.displayName, StringComparison.OrdinalIgnoreCase));
            return list;
        }

        internal IEnumerable<(string path, string name)> Collate() {
            return this.Keywords.SelectMany(node => node.Collapse());
        }

        private void OnValidate() {
            Stack<string> path = new Stack<string>();
            DepthFirstWalker<KeywordSheetNode> walker = new DepthFirstWalker<KeywordSheetNode>(
                onVisit: node => {
                    node.Path = string.Join('/', path.Reverse());
                },
                onBacktrack: (curr, prev) => {
                    path.Pop();
                },
                onMoveForward: (curr, next) => {
                    path.Push(next.Name);
                }
            );

            foreach (KeywordSheetNode node in this.Keywords) {
                if (node is null) {
                    continue;
                }

                path.Clear();
                path.Push(node.Name);
                walker.Iterate(this, node);
            }
        }
    }
}