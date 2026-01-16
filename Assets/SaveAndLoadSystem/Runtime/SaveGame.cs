using System;
using System.Collections.Generic;
using System.Linq;
using SaveAndLoadSystem.Runtime.Momentos;
using UnityEngine;

namespace SaveAndLoadSystem.Runtime {
    [Serializable]
    public sealed class SaveGame {
        [field: SerializeField] internal string Filename { get; set; } = string.Empty;
        [field: SerializeReference] internal List<IMomento> SavedData { get; set; } = new List<IMomento>();

        private Lazy<IDictionary<string, IMomento>> Momentos { get; }

        internal IMomento this[string id] {
            get => this.Momentos.Value[id];
            set => this.Momentos.Value[id] = value;
        }

        public SaveGame() {
            this.Momentos = new Lazy<IDictionary<string, IMomento>>(this.CacheMomentos);
        }

        private IDictionary<string, IMomento> CacheMomentos() {
            return this.SavedData.ToDictionary(momento => momento.Id, momento => momento);
        }
        
        internal S ReadSaveData<S>(string id) where S : IMomento, new() {
            if (this.Momentos.Value.TryGetValue(id, out IMomento momento) && momento is S data) {
                return data;
            }
            
            data = new S { Id = id };
            this.Momentos.Value[id] = data;
            this.SavedData.Add(data);
            return data;
        }
    }
}
