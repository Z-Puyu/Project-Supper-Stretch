using System.Collections;
using System.Collections.Generic;
using CommonFrameworks.Trees;
using UnityEngine;

namespace GameplayKeywordsSystem.Runtime {
    [DisallowMultipleComponent]
    public abstract class TaggableMonoBehaviour : MonoBehaviour, ITaggable<Keyword>, ICollection<Keyword> {
        [field: SerializeField] private KeywordLabel Keywords { get; set; } = new KeywordLabel();
        
        public int Count => this.Keywords.Count;
        public bool IsReadOnly => this.Keywords.IsReadOnly;
        
        protected virtual void Awake() {
            this.Keywords.Initialise();
        }
        
        public bool Add(Keyword label) {
            return this.Keywords.Add(label);
        }

        public void CopyTo(Keyword[] array, int arrayIndex) {
            this.Keywords.CopyTo(array, arrayIndex);
        }

        public bool Remove(Keyword label) {
            return this.Keywords.Remove(label);
        }

        public bool Contains(Keyword label) {
            return this.Keywords.Contains(label);
        }

        void ICollection<Keyword>.Add(Keyword item) {
            this.Keywords.Add(item);
        }

        public void Clear() {
            this.Keywords.Clear();
        }

        public IEnumerator<Keyword> GetEnumerator() {
            return this.Keywords.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
