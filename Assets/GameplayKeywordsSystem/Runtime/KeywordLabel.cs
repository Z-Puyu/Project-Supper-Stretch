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
            this.Keywords.Clear();
        }

        public bool Contains(Keyword item) {
            return this.Keywords.ContainsPrefix(item);
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

        public bool Remove(Keyword item) {
            if (!this.Keywords.RemoveAllWithPrefix(item, out IEnumerable<Keyword> removed)) {
                return false;
            }

            foreach (Keyword keyword in removed) {
                this.OnKeywordRemoved.Invoke(keyword);
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