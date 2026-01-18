using System;
using UnityEngine;

namespace SaveAndLoadSystem.Runtime {
    [Serializable]
    public record struct SaveSlot {
        [field: SerializeField] internal int Index { get; private set; }
        [field: SerializeField] internal string Name { get; private set; }

        internal SaveSlot(int index, string name) {
            this.Index = index;
            this.Name = name;
        }
        
        internal SaveSlot(int index) : this(index, string.Empty) { }
    }
}
