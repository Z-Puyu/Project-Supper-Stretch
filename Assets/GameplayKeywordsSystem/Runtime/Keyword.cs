using System;
using System.Collections;
using System.Collections.Generic;

namespace GameplayKeywordsSystem.Runtime {
    public readonly record struct Keyword(string Value) : IComparable<Keyword>, IEnumerable<char> {
        public static readonly Keyword Empty = new Keyword(string.Empty);

        public Keyword Chop(int length = 1) {
            if (length <= 0) {
                return new Keyword(this.Value);
            }
            
            string[] parts = this.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return length >= parts.Length
                    ? Keyword.Empty
                    : new Keyword(string.Join("/", parts, 0, parts.Length - length));
        }
        
        public int CompareTo(Keyword other) {
            return string.Compare(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() {
            return this.Value?.GetHashCode() ?? 0;
        }

        public IEnumerator<char> GetEnumerator() {
            return this.Value.GetEnumerator();
        }

        public override string ToString() {
            return this.Value.Trim().ToLower();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public static implicit operator string(Keyword keyword) {
            return keyword.Value;
        }
        
        public static implicit operator Keyword(string value) {
            return new Keyword(value);
        }

        public static bool operator ==(Keyword keyword, string str) {
            return string.Equals(keyword.Value, str, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(Keyword keyword, string str) {
            return !string.Equals(keyword.Value, str, StringComparison.OrdinalIgnoreCase);
        }
    }
}