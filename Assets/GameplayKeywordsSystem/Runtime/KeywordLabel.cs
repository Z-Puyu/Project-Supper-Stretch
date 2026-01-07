using System;
using System.Collections;
using System.Collections.Generic;
using CommonFrameworks.Collections;
using SaintsField;
using UnityEngine;

namespace GameplayKeywordsSystem.Runtime {
    [Serializable]
    public sealed class KeywordLabel : ITaggable<Keyword>, ICollection<Keyword> {
        private TrieSet<Keyword, char> Keywords { get; } = new TrieSet<Keyword, char>('/');
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords)), DefaultExpand] 
        private List<string> PreexistingKeywords { get; set; } = new List<string>();
        
        public event Action<Keyword> OnKeywordAdded = delegate { };
        public event Action<Keyword> OnKeywordRemoved = delegate { };
        
        public int Count => this.Keywords.Count;
        public bool IsReadOnly => this.Keywords.IsReadOnly;
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.GetTreeDropdownList();
        
        internal void Initialise() {
            foreach (string keyword in this.PreexistingKeywords) {
                this.Keywords.Add(keyword);
            }
        }

        void ICollection<Keyword>.Add(Keyword item) {
            this.Add(item);
        }

        public void Clear() {
            foreach (Keyword keyword in this.Keywords) {
                this.OnKeywordRemoved.Invoke(keyword);
            }
            
            this.Keywords.Clear();
        }

        /// <summary>
        /// Checks if the label contains the given keyword.
        /// </summary>
        /// <param name="keyword">The keyword to check for.</param>
        /// <returns><c>true</c> if the keyword is a prefix of any keyword present in the label</returns>
        public bool Contains(Keyword keyword) {
            return this.Keywords.ContainsPrefix(keyword);
        }

        public void CopyTo(Keyword[] array, int arrayIndex) {
            this.Keywords.CopyTo(array, arrayIndex);
        }

        public bool Add(Keyword label) {
            if (!this.Keywords.Add(label)) {
                return false;
            }

            this.OnKeywordAdded.Invoke(label);
            return true;
        }

        /// <summary>
        /// Removes the given keyword from the label.
        /// </summary>
        /// <param name="keyword">The keyword to remove</param>
        /// <returns><c>true</c> if the keyword is removed, and <c>false</c> if the keyword is not present</returns>
        /// <remarks>
        /// This removes all keywords that start with the given keyword.
        /// </remarks>
        public bool Remove(Keyword keyword) {
            if (!this.Keywords.RemoveAllWithPrefix(keyword, out IEnumerable<Keyword> removed)) {
                return false;
            }

            foreach (Keyword k in removed) {
                this.OnKeywordRemoved.Invoke(k);
            }
                
            return true;
        }

        public IEnumerator<Keyword> GetEnumerator() {
            return this.Keywords.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}