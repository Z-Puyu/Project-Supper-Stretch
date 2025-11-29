using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Trees;
using SaintsField;
using UnityEngine;

namespace GameplayKeywordsSystem.Runtime {
    [Serializable]
    public sealed class KeywordLabel : ITaggable<Keyword>, ICollection<Keyword> {
        private TrieSet<Keyword, char> Tags { get; } = new TrieSet<Keyword, char>('.');
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))] 
        private List<string> PreexistingKeywords { get; set; } = new List<string>();
        
        public int Count => this.Tags.Count;
        public bool IsReadOnly => this.Tags.IsReadOnly;
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.GetDropdownList();
        
        internal void Initialise() {
            foreach (string keyword in this.PreexistingKeywords) {
                this.Tags.Add(keyword);
            }
        }

        void ICollection<Keyword>.Add(Keyword item) {
            this.Tags.Add(item);
        }

        public void Clear() {
            this.Tags.Clear();
        }

        public bool Contains(Keyword item) {
            return this.Tags.ContainsPrefix(item);
        }

        public void CopyTo(Keyword[] array, int arrayIndex) {
            this.Tags.CopyTo(array, arrayIndex);
        }

        public bool Add(Keyword label) {
            return this.Tags.Add(label);
        }

        public bool Remove(Keyword item) {
            return this.Tags.RemoveAllWithPrefix(item);
        }

        public IEnumerator<Keyword> GetEnumerator() {
            return this.Tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
