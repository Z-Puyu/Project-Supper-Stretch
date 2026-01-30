using System;
using SaintsField;
using UnityEngine;

namespace GameplaySensors.Runtime {
    [Serializable]
    public abstract class PhysicsCaster : IPhysicsCaster {
        [field: SerializeField, MinValue(0.01f)] 
        private float MaxDistance { get; set; } = 100;
        
        [field: SerializeField] private LayerMask IgnoredLayers { get; set; }
        [field: SerializeField] private LayerMask CheckedLayers { get; set; }

        protected abstract bool Cast(
            Vector3 origin, Vector3 direction, float distance, LayerMask mask, out RaycastHit hit
        );
        
        protected abstract bool Cast(
            Vector3 origin, Vector3 direction, float distance, LayerMask mask, out RaycastHit[] hits
        );

        public bool CastHit(Vector3 origin, Vector3 direction, float distance, LayerMask mask, out RaycastHit hit) {
            LayerMask layers = (mask | this.CheckedLayers) & ~this.IgnoredLayers;
            return this.Cast(origin, direction, Math.Min(distance, this.MaxDistance), layers, out hit);    
        }

        public bool CastHit(Vector3 origin, Vector3 direction, float distance, LayerMask mask, out RaycastHit[] hits) {
            LayerMask layers = (mask | this.CheckedLayers) & ~this.IgnoredLayers;
            return this.Cast(origin, direction, Math.Min(distance, this.MaxDistance), layers, out hits);
        }
    }
}
