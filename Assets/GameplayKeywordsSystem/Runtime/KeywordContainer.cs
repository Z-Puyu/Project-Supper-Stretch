using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Flags;
using CommonFrameworks.Trees;
using UnityEngine;

namespace GameplayKeywordsSystem.Runtime {
    [DisallowMultipleComponent]
    public sealed class KeywordContainer : TrieSetComponent<Keyword, char>, IFlag<string>, IFlag<Keyword> {
        public KeywordContainer() : base(new TrieSet<Keyword, char>('.')) { }

        public bool Has(string flag) {
            return this.Has(new Keyword(flag));
        }

        public void Set(string item) {
            this.Set(new Keyword(item));
        }

        public void Unset(string item) {
            this.Unset(new Keyword(item));
        }

        public void Toggle(string item) {
            this.Toggle(new Keyword(item));
        }

        public bool Has(Keyword flag) {
            return this.Contains(flag);
        }

        public void Set(Keyword item) {
            this.Add(item);
        }

        public void Unset(Keyword item) {
            this.Remove(item);
        }

        public void Toggle(Keyword item) {
            if (this.Contains(item)) {
                this.Remove(item);
            } else {
                this.Add(item);
            }
        }

        public IEnumerable<Keyword> GetAllPresent() {
            return this;
        }

        public bool HasAnyPresent(out Keyword first) {
            if (this.Count == 0) {
                first = default;
                return false;
            }
            
            first = this.First();
            return true;
        }

        IEnumerable<string> IFlag<string>.GetAllPresent() {
            return this.Select(keyword => keyword.ToString());
        }

        public bool HasAnyPresent(out string first) {
            if (this.Count == 0) {
                first = null;
                return false;
            }
            
            first = this.First();
            return true;
        }
    }
}
