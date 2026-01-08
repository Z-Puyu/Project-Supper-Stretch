using System;
using System.Diagnostics.CodeAnalysis;
using Characters.Events;
using CommonFrameworks.Events;
using CommonFrameworks.Utilities;
using GameCharacterBehaviours.Runtime.Movement;
using GameplayAbilitiesSystem.Runtime.Attributes;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Animations;

namespace Characters.Player {
    [DisallowMultipleComponent, RequireComponent(typeof(Locomotion))]
    public class PlayerMovementInterpreter : MonoBehaviour {
        [NotNull] private Locomotion? Locomotion { get; set; }
        
        [NotNull] 
        [field: SerializeField, Required] 
        private Animator? Animator { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
        [field: ShowIf(nameof(this.Animator)), Required]
        private int LeftRightVelocityAnimatorParameter { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
        [field: ShowIf(nameof(this.Animator)), Required]
        private int ForwardBackVelocityAnimatorParameter { get; set; }
        
        [field: SerializeField, MinValue(0)] private float AnimationBlendTime { get; set; } = 0.1f;

        private void Awake() {
            this.Locomotion = this.GetComponent<Locomotion>();
        }

        private void OnEnable() {
            this.Subscribe<PlayerInputInterpreter, PerformSprintingMessage>(this.HandleSprintEvent);
        }
        
        private void OnDisable() {
            this.Mute();
        }
        
        private void HandleSprintEvent(Event<PlayerInputInterpreter, PerformSprintingMessage> @event) {
            this.Locomotion.Mode = @event.Message.IsSprinting ? Locomotion.Gesture.Sprint : Locomotion.Gesture.Run;
        }

        private void Update() {
            Vector2 input = Singleton<PlayerInputInterpreter>.Instance.MovementInput * this.Locomotion.CurrentSpeed;
            Vector3 direction = CameraSystem.PlanarForward * input.y + CameraSystem.PlanarRight * input.x;
            this.Locomotion.IsMoving = input.sqrMagnitude >= 0.0001;
            
            this.Animator.SetFloat(
                this.LeftRightVelocityAnimatorParameter, input.x, this.AnimationBlendTime, Time.deltaTime
            );
            
            this.Animator.SetFloat(
                this.ForwardBackVelocityAnimatorParameter, input.y, this.AnimationBlendTime, Time.deltaTime
            );
            
            this.Locomotion.PlanarDirection = new Vector2(direction.x, direction.z).normalized;
        }
    }
}