using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.PhysicalProperties {
    [DisallowMultipleComponent]
    public sealed class Mass : MonoBehaviour {
        [field: SerializeField, MinValue(0), EndText("kg")] 
        private float Value { get; set; } = 1;
        
        public float TotalMass { get; private set; }
        public Vector3 Weight => Physics.gravity * this.TotalMass;

        private void OnEnable() {
            foreach (Mass mass in this.GetComponentsInParent<Mass>(true)) {
                mass.TotalMass += this.Value;   
            }
        }
        
        private void OnDisable() {
            foreach (Mass mass in this.GetComponentsInParent<Mass>(true)) {
                mass.TotalMass -= this.Value;   
            }
        }

        private void OnBeforeTransformParentChanged() {
            this.enabled = false;
        }

        private void OnTransformParentChanged() {
            this.enabled = true;
        }
    }
}
