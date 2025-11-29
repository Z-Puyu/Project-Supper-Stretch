using System;
using System.Collections;
using System.Collections.Generic;

namespace GameplayKeywordsSystem.Runtime {
    public readonly struct Keyword : IComparable<Keyword>, IEquatable<Keyword>, IEnumerable<char> {
        private string Value { get; }
        
        public Keyword(string value) {
            this.Value = value;
        }

        public int CompareTo(Keyword other) {
            return string.Compare(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(Keyword other) {
            return string.Equals(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }
        
        public override bool Equals(object obj) {
            return obj is Keyword other && this.Equals(other);
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
        
        public static bool operator ==(Keyword keyword, Keyword other) {
            return string.Equals(keyword.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(Keyword keyword, Keyword other) {
            return !string.Equals(keyword.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
