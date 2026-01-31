using System;
using SaveAndLoad;
using SaveAndLoad.Momentos;
using UnityEngine;

namespace Characters.Player {
    public sealed class PlayerSaveDataProcessor : SaveDataProcessor<Transform, PlayerSaveDataProcessor.PlayerSaveData> {
        [Serializable]
        public sealed class PlayerSaveData : IMomento<Transform> {
            [field: SerializeField] public string Id { get; set; } = string.Empty;
            [field: SerializeField] public TransformData Transform { get; private set; } = new TransformData();
            
            public void Capture(Transform transform) {
                this.Transform.Save(transform);
            }
            
            public void Restore(Transform transform) {
                this.Transform.Load(transform);
            }
        }
    }
}
