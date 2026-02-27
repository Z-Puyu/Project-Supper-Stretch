using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilities.Utils {
    [Serializable]
    internal class SerialisableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver {
        [field: SerializeField] private List<Entry> Entries { get; set; } = new List<Entry>();

        public void OnBeforeSerialize() {
            this.Clear();
            foreach (Entry e in this.Entries) {
                this.Add(e.Key, e.Value);
            }
        }

        public void OnAfterDeserialize() {
            this.Entries.Clear();
            foreach ((K key, V value) in this) {
                this.Entries.Add(new Entry { Key = key, Value = value });
            }
        }

        [Serializable]
        private record struct Entry {
            [field: SerializeField] internal K Key { get; set; }
            [field: SerializeField] internal V Value { get; set; }
        }
    }
}
