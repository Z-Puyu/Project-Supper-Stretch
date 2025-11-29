using System;
using System.Collections;
using System.Collections.Generic;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    public readonly struct AttributeKey : IComparable<AttributeKey>, IEquatable<AttributeKey>, IEnumerable<char> {
        private string Value { get; }
        
        public AttributeKey(string value) {
            this.Value = value;
        }

        public int CompareTo(AttributeKey other) {
            return string.Compare(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(AttributeKey other) {
            return string.Equals(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }
        
        public override bool Equals(object obj) {
            return obj is AttributeKey other && this.Equals(other);
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

        public static implicit operator string(AttributeKey key) {
            return key.Value;
        }
        
        public static implicit operator AttributeKey(string value) {
            return new AttributeKey(value);
        }

        public static bool operator ==(AttributeKey key, string id) {
            return string.Equals(key.Value, id, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(AttributeKey key, string id) {
            return !string.Equals(key.Value, id, StringComparison.OrdinalIgnoreCase);
        }
        
        public static bool operator ==(AttributeKey key, AttributeKey other) {
            return string.Equals(key.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(AttributeKey key, AttributeKey other) {
            return !string.Equals(key.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
