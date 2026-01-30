using UnityEngine;

namespace GameplaySensors.Runtime {
    public interface IPhysicsCaster {
        public bool CastHit(Vector3 origin, Vector3 direction, float distance, LayerMask mask, out RaycastHit hit);
        public bool CastHit(Vector3 origin, Vector3 direction, float distance, LayerMask mask, out RaycastHit[] hits);
    }
}
