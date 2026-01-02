using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    [DisallowMultipleComponent, RequireComponent(typeof(Animator))]
    public sealed class RootMotion : MonoBehaviour {
        [NotNull] 
        [field: SerializeField, Required] 
        private Locomotion? MovementController { get; set; }
    
        [NotNull] private Animator? Animator { get; set; }

        private void Awake() {
            this.Animator = this.GetComponent<Animator>();
        }

        private void OnAnimatorMove() {
            this.MovementController.MoveAndRotate(this.Animator.deltaPosition);
        }
    }
}