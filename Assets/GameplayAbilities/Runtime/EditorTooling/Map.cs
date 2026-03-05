using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilities.Runtime.EditorTooling {
    [Serializable]
    public class Map<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver {
        [field: SerializeField] private List<Entry> Entries { get; set; } = new List<Entry>();
        [field: SerializeField] private string KeyLabel { get; set; }
        [field: SerializeField] private string ValueLabel { get; set; }

        public Map(string keyLabel = "", string valueLabel = "") {
            this.KeyLabel = keyLabel;
            this.ValueLabel = valueLabel;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            if (this.Count == 0) {
                return;
            }

            this.Entries.Clear();
            foreach ((K key, V value) in this) {
                this.Entries.Add(new Entry { Key = key, Value = value });
            }
        }
        
        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            this.Clear();
            foreach (Entry e in this.Entries) {
                if (e.Key is null) {
                    continue;
                }
        
                this.Add(e.Key, e.Value);
            }
        }

        [Serializable]
        private record struct Entry {
            [field: SerializeField] internal K Key { get; set; }
            [field: SerializeField] internal V Value { get; set; }
        }
    }
}
