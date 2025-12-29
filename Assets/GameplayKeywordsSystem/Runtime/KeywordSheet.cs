using System;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Iterators;
using CommonFrameworks.Trees;
using SaintsField;
using UnityEngine;

namespace GameplayKeywordsSystem.Runtime {
    [CreateAssetMenu(fileName = "New Keyword Sheet", menuName = "Gameplay Keywords/Keyword Sheet")]
    public sealed class KeywordSheet : ScriptableObject, ITraversable<KeywordSheetNode> {
        [field: SerializeField, DefaultExpand, FieldDefaultExpand] 
        private List<KeywordSheetNode> Keywords { get; set; } = new List<KeywordSheetNode>();

        KeywordSheetNode ITraversable<KeywordSheetNode>.Start => this.Keywords.FirstOrDefault();

        bool ITraversable<KeywordSheetNode>.HasOutNeighbours(KeywordSheetNode vertex, out IEnumerable<KeywordSheetNode> children) {
            children = vertex.Children;
            return vertex.Children.Count > 0;
        }

        private bool Contains(string keyword) {
            string[] parts = keyword.Trim().ToLower().Split('.', StringSplitOptions.RemoveEmptyEntries);
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

        internal IEnumerable<AdvancedDropdownList<string>> ToDropdownLists() {
            return this.Keywords.Select(node => node.ToDropdownList());
        }

        private void OnValidate() {
            Stack<string> path = new Stack<string>();
            DepthFirstWalker<KeywordSheetNode> walker = new DepthFirstWalker<KeywordSheetNode>(
                onVisit: node => node.Path = string.Join('.', path.Reverse()),
                onBacktrack: (_, _) => path.Pop(),
                onMoveForward: (_, next) => path.Push(next.Name)
            );
            
            foreach (KeywordSheetNode node in this.Keywords) {
                path.Clear();
                path.Push(node.Name);
                walker.Iterate(this, node);
            }
        }
    }
}