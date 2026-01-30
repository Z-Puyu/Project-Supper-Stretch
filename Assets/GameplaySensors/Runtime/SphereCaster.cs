using System;
using SaintsField;
using UnityEngine;

namespace GameplaySensors.Runtime {
    [Serializable]
    internal sealed class SphereCaster : PhysicsCaster {
        [field: SerializeField, MinValue(0.01f)] 
        private float Radius { get; set; } = 1;

        protected override bool Cast(
            Vector3 origin, Vector3 direction, float distance, LayerMask mask, out RaycastHit hit
        ) {
            return Physics.SphereCast(
                origin, this.Radius, direction, out hit, distance, mask, QueryTriggerInteraction.Ignore
            );
        }

        protected override bool Cast(
            Vector3 origin, Vector3 direction, float distance, LayerMask mask, out RaycastHit[] hits
        ) {
            hits = Physics.SphereCastAll(
                origin, this.Radius, direction, distance, mask, QueryTriggerInteraction.Ignore
            );

            return hits.Length > 0;
        }
    }
}
