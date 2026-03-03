using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace GameplayAbilities.Abilities {
    public sealed class AbilityExecutionUserData : IReadOnlyDictionary<string, double> {
        private static readonly ObjectPool<AbilityExecutionUserData> Pool = new ObjectPool<AbilityExecutionUserData>(
            () => new AbilityExecutionUserData(), defaultCapacity: 20, maxSize: 100
        );

        private IDictionary<string, double> Entries { get; } = new Dictionary<string, double>();
        
        public double this[string key] => this.Entries[key];

        public int Count => this.Entries.Count;
        public IEnumerable<string> Keys => this.Entries.Keys;
        public IEnumerable<double> Values => this.Entries.Values;

        private AbilityExecutionUserData() { }

        public static AbilityExecutionUserData New() {
            return AbilityExecutionUserData.Pool.Get();
        }

        public AbilityExecutionUserData With(string key, double value) {
            this.Entries[key] = value;
            return this;
        }

        public AbilityExecutionUserData Less(string key) {
            this.Entries.Remove(key);
            return this;
        }

        public bool ContainsKey(string key) {
            return this.Entries.ContainsKey(key);
        }

        public bool TryGetValue(string key, out double value) {
            return this.Entries.TryGetValue(key, out value);
        }
        
        public IEnumerator<KeyValuePair<string, double>> GetEnumerator() {
            return this.Entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
        
        internal void Dispose() {
            this.Entries.Clear();
            AbilityExecutionUserData.Pool.Release(this);
        }
    }
}
