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
        }

        public bool Tag(Keyword label) {
            return this.Label.Tag(label);
        }

        public void CopyTo(Keyword[] array, int arrayIndex) {
            this.Label.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the given keyword from the label.
        /// </summary>
        /// <param name="keyword">The keyword to remove</param>
        /// <returns><c>true</c> if the keyword is removed, and <c>false</c> if the keyword is not present</returns>
        /// <remarks>
        /// This removes all keywords that start with the given keyword.
        /// </remarks>
        public bool Untag(Keyword keyword) {
            return this.Label.Untag(keyword);
        }

        /// <summary>
        /// Checks if the label contains the given keyword.
        /// </summary>
        /// <param name="keyword">The keyword to check for.</param>
        /// <returns><c>true</c> if the keyword is a prefix of any keyword present in the label</returns>
        public bool HasTag(Keyword keyword) {
            return this.Label.HasTag(keyword);
        }

        void ICollection<Keyword>.Add(Keyword item) {
            this.Label.Tag(item);
        }
        
        bool ICollection<Keyword>.Remove(Keyword item) {
            return this.Untag(item);
        }
        
        bool ICollection<Keyword>.Contains(Keyword item) {
            return this.HasTag(item);
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