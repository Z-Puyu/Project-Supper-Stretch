using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Collections;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayKeywordsSystem.Runtime {
    [Serializable]
    public sealed class KeywordLabel : ITaggable<Keyword>, ICollection<Keyword> {
        private TrieSet<Keyword, char> Keywords { get; } = new TrieSet<Keyword, char>('/');

        private TrieDictionary<Keyword, char, EventTrigger> Events { get; } =
            new TrieDictionary<Keyword, char, EventTrigger>();
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords)), DefaultExpand] 
        private List<string> PreexistingKeywords { get; set; } = new List<string>();
        
        [field: SerializeField, Table] private List<KeywordEventTrigger> OnAddKeywordEvents { get; set; } = new List<KeywordEventTrigger>();
        [field: SerializeField, Table] private List<KeywordEventTrigger> OnRemoveKeywordEvents { get; set; } = new List<KeywordEventTrigger>();
        [field: SerializeField] private UnityEvent<Keyword> OnAnyKeywordAdded { get; set; } = new UnityEvent<Keyword>();
        [field: SerializeField] private UnityEvent<Keyword> OnAnyKeywordRemoved { get; set; } = new UnityEvent<Keyword>();
        
        public int Count => this.Keywords.Count;
        public bool IsReadOnly => this.Keywords.IsReadOnly;
        
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.Fetch<KeywordSheet>();
        
        internal void Initialise() {
            foreach (string keyword in this.PreexistingKeywords) {
                this.Keywords.Add(keyword);
            }

            IEnumerable<string> observed = this.OnAddKeywordEvents
                                               .Concat(this.OnRemoveKeywordEvents)
                                               .Select(@event => @event.Keyword)
                                               .Distinct();
            Dictionary<string, UnityEvent> onAdd = this.OnAddKeywordEvents.ToDictionary(
                trigger => trigger.Keyword, trigger => trigger.Event
            );
            
            Dictionary<string, UnityEvent> onRemove = this.OnRemoveKeywordEvents.ToDictionary(
                trigger => trigger.Keyword, trigger => trigger.Event
            );
            
            foreach (string keyword in observed) {
                this.Events.Add(
                    keyword, new EventTrigger(onAdd.GetValueOrDefault(keyword), onRemove.GetValueOrDefault(keyword))
                );
            }
        }

        void ICollection<Keyword>.Add(Keyword item) {
            this.Tag(item);
        }
        
        bool ICollection<Keyword>.Remove(Keyword item) {
            return this.Untag(item);
        }
        
        bool ICollection<Keyword>.Contains(Keyword item) {
            return this.HasTag(item);
        }

        private void TriggerEvents(Keyword keyword, bool removed) {
            if (removed) {
                this.OnAnyKeywordRemoved.Invoke(keyword);
            } else {
                this.OnAnyKeywordAdded.Invoke(keyword);
            }
            
            Keyword prefix = string.Empty;
            while (this.Events.FindLongestPrefixKey(keyword, out KeyValuePair<Keyword, EventTrigger> trigger)) {
                keyword = keyword.Chop();
                if (prefix == trigger.Key) {
                    continue;
                }
                
                prefix = trigger.Key;
                if (removed) {
                    trigger.Value.OnKeywordRemoved?.Invoke();
                } else {
                    trigger.Value.OnKeywordAdded?.Invoke();
                }
            }
        }

        public void Clear() {
            foreach (Keyword keyword in this.Keywords) {
                this.TriggerEvents(keyword, true);
            }
            
            this.Keywords.Clear();
        }

        /// <summary>
        /// Checks if the label contains the given keyword.
        /// </summary>
        /// <param name="keyword">The keyword to check for.</param>
        /// <returns><c>true</c> if the keyword is a prefix of any keyword present in the label</returns>
        public bool HasTag(Keyword keyword) {
            return this.Keywords.ContainsPrefix(keyword);
        }

        public void CopyTo(Keyword[] array, int arrayIndex) {
            this.Keywords.CopyTo(array, arrayIndex);
        }

        public bool Tag(Keyword label) {
            if (!this.Keywords.Add(label)) {
                return false;
            }

            this.TriggerEvents(label, false);
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
        public bool Untag(Keyword keyword) {
            if (!this.Keywords.RemoveAllWithPrefix(keyword, out IEnumerable<Keyword> removed)) {
                return false;
            }

            foreach (Keyword k in removed) {
                this.TriggerEvents(k, true);
            }
                
            return true;
        }

        public IEnumerator<Keyword> GetEnumerator() {
            return this.Keywords.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        private readonly record struct EventTrigger(UnityEvent? OnKeywordAdded, UnityEvent? OnKeywordRemoved);
    }
}