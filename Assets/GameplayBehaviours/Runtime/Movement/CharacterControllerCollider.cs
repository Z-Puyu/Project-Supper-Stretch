using UnityEngine;
using UnityEngine.Events;

namespace GameplayBehaviours.Movement {
    [DisallowMultipleComponent, RequireComponent(typeof(CharacterController)), AddComponentMenu("")]
    internal sealed class CharacterControllerCollider : MonoBehaviour {
        internal event UnityAction OnGrounded = delegate { };
        internal event UnityAction<ControllerColliderHit> OnCollision = delegate { };

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            this.OnCollision.Invoke(hit);
            if (hit.controller.isGrounded) {
                this.OnGrounded.Invoke();
            }
        }
    }
}
