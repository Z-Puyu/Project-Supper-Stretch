using System;
using System.Collections;
using System.Collections.Generic;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    public readonly record struct AttributeKey(string Value) : IComparable<AttributeKey>, IEnumerable<char> {
        public static readonly AttributeKey Empty = new AttributeKey(string.Empty);
        
        public int CompareTo(AttributeKey other) {
            return string.Compare(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
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
    }
}