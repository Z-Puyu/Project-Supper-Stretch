using System;
using System.Collections;
using System.Collections.Generic;
using CommonFrameworks.Components;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayKeywordsSystem.Runtime {
    [DisallowMultipleComponent]
    public sealed class KeywordContainer : BehaviourComponent, ITaggable<Keyword>, ICollection<Keyword> {
        [field: SerializeField, SaintsRow(inline: true)] 
        private KeywordLabel Label { get; set; } = new KeywordLabel();
        
        [field: SerializeField] private UnityEvent<Keyword> OnKeywordAdded { get; set; } = new UnityEvent<Keyword>();
        [field: SerializeField] private UnityEvent<Keyword> OnKeywordRemoved { get; set; } = new UnityEvent<Keyword>();
        
        public int Count => this.Label.Count;
        public bool IsReadOnly => this.Label.IsReadOnly;
        
        protected override void Awake() {
            base.Awake();
            this.Label.Initialise();
            this.Label.OnKeywordRemoved += this.OnKeywordRemoved.Invoke;
            this.Label.OnKeywordAdded += this.OnKeywordAdded.Invoke;
        }

        private void OnDestroy() {
            this.Label.OnKeywordRemoved -= this.OnKeywordRemoved.Invoke;
            this.Label.OnKeywordAdded -= this.OnKeywordAdded.Invoke;
        }

        public bool Add(Keyword label) {
            return this.Label.Add(label);
        }

        public void CopyTo(Keyword[] array, int arrayIndex) {
            this.Label.CopyTo(array, arrayIndex);
        }

        public bool Remove(Keyword label) {
            return this.Label.Remove(label);
        }

        public bool Contains(Keyword label) {
            return this.Label.Contains(label);
        }

        void ICollection<Keyword>.Add(Keyword item) {
            this.Label.Add(item);
        }

        public void Clear() {
            this.Label.Clear();
        }

        public IEnumerator<Keyword> GetEnumerator() {
            return this.Label.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}