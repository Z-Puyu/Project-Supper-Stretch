using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace GameCharacterBehaviours.Runtime.Movement {
    [DisallowMultipleComponent, RequireComponent(typeof(Animator)), AddComponentMenu("")]
    public sealed class RootMotion : MonoBehaviour {
        [NotNull] internal Locomotion? MovementController { private get; set; }
        [NotNull] private Animator? Animator { get; set; }

        private void Awake() {
            this.Animator = this.GetComponent<Animator>();
        }

        private void OnEnable() {
            this.MovementController.UseRootMotion = true;
        }
        
        private void OnDisable() {
            this.MovementController.UseRootMotion = false;
            this.Animator.applyRootMotion = false;
        }

        private void OnAnimatorMove() {
            if (this.MovementController.IsMoving) { 
                this.MovementController.MoveBy(this.Animator.deltaPosition);
            }
        }
    }
}