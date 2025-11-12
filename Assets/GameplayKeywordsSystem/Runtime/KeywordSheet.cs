using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.CommonUtilities.Databases;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayKeywordsSystem.Runtime {
    [CreateAssetMenu(fileName = "New Keyword Sheet", menuName = "Gameplay Keywords/Keyword Sheet")]
    public sealed class KeywordSheet : ScriptableObject, IEnumerable<Keyword> {
        [field: SerializeField]
        private List<KeywordSheetNode> Keywords { get; set; } = new List<KeywordSheetNode>();

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
        
        [Button]
        private void AddKeyword(string keyword) {
            keyword = keyword.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(keyword)) {
                Debug.LogWarning("Keyword cannot be empty.");
                return;
            }

            KeywordSheet duplicate = Database<KeywordSheet>.LoadedResources
                                                           .FirstOrDefault(sheet => sheet.Contains(keyword));
            if (duplicate) {
                Debug.LogWarning($"Keyword '{keyword}' already exists in sheet {duplicate.name}.");
                return;
            }
            
            string[] parts = keyword.Trim().ToLower().Split('.', StringSplitOptions.RemoveEmptyEntries);
            List<string> path = new List<string> { parts[0] };
            KeywordSheetNode root = this.Keywords.Find(node => node.Name == parts[0]);
            if (root is null) {
                root = new KeywordSheetNode(path[0], path[0]);
                this.Keywords.Add(root);
            }

            KeywordSheetNode curr = root;
            for (int i = 1; i < parts.Length; i += 1) {
                path.Add(parts[i]);
                curr = curr.FindOrAddChild(path);
            }
            
            this.Keywords.Sort();
        }

        internal IEnumerable<AdvancedDropdownList<string>> ToDropdownLists() {
            return this.Keywords.Select(node => node.ToDropdownList());
        }

        public IEnumerator<Keyword> GetEnumerator() {
            Queue<KeywordSheetNode> queue = new Queue<KeywordSheetNode>(this.Keywords);
            while (queue.TryDequeue(out KeywordSheetNode node)) {
                if (node.IsLeaf) {
                    yield return node.Path;
                } else {
                    foreach (KeywordSheetNode child in node.Children) {
                        queue.Enqueue(child);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
