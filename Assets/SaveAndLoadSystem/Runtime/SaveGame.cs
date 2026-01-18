using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SaveAndLoadSystem.Runtime.Momentos;
using UnityEngine;
using UnityEngine.Serialization;

namespace SaveAndLoadSystem.Runtime {
    [Serializable]
    public sealed class SaveGame : IComparable<SaveGame> {
        [SerializeField] internal Metadata metadata;
        [SerializeReference] internal List<IMomento> data = new List<IMomento>();
        
        public DateTime Timestamp => DateTime.Parse(this.metadata.Timestamp);
        private Lazy<IDictionary<string, IMomento>> Momentos { get; }

        internal IMomento this[string id] {
            get => this.Momentos.Value[id];
            set => this.Momentos.Value[id] = value;
        }

        internal SaveGame(Metadata metadata) {
            this.metadata = metadata;
            this.Momentos = new Lazy<IDictionary<string, IMomento>>(this.CacheMomentos);
        }

        internal static SaveGame Create(SaveSlot slot) {
            Metadata metadata = new Metadata(slot);
            return new SaveGame(metadata);
        }

        private IDictionary<string, IMomento> CacheMomentos() {
            return this.data.ToDictionary(momento => momento.Id, momento => momento);
        }
        
        internal S ReadSaveData<S>(string id) where S : IMomento, new() {
            if (this.Momentos.Value.TryGetValue(id, out IMomento momento) && momento is S data) {
                return data;
            }
            
            data = new S { Id = id };
            this.Momentos.Value[id] = data;
            this.data.Add(data);
            return data;
        }
        
        public int CompareTo(SaveGame? other) {
            return other is null ? 1 : this.Timestamp.CompareTo(other.Timestamp);
        }

        [Serializable]
        internal record struct Metadata {
            [field: SerializeField] internal string SaveFilePath { get; private set; }
            [field: SerializeField] internal SaveSlot Slot { get; private set; }
            [field: SerializeField] internal string DisplayName { get; private set; }
            [field: SerializeField] internal string Timestamp { get; private set; }
            
            internal Metadata(SaveSlot slot, string path = "", string name = "", string timestamp = "") {
                this.Slot = slot;
                this.SaveFilePath = path;
                this.DisplayName = name;
                this.Timestamp = timestamp;
            }
        }
    }
}
