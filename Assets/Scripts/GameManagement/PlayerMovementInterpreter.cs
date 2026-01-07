using System;
using System.Diagnostics.CodeAnalysis;
using Characters;
using Characters.Player;
using CommonFrameworks.Utilities;
using GameCharacterBehaviours.Runtime.Movement;
using GameplayAbilitiesSystem.Runtime.Attributes;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using Attribute = Codice.Client.BaseCommands.Attribute;

namespace GameManagement {
    [DisallowMultipleComponent, RequireComponent(typeof(Locomotion))]
    public class PlayerMovementInterpreter : MonoBehaviour {
        [NotNull] private Locomotion? Locomotion { get; set; }
        [NotNull] private AttributeSet? AttributeSet { get; set; }
        
        [NotNull] 
        [field: SerializeField, Required] 
        private Animator? Animator { get; set; }
        
        [field: SerializeField, TreeDropdown(nameof(this.AllAttributes))] 
        private string MovementSpeedAttribute { get; set; } = string.Empty;
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
        [field: ShowIf(nameof(this.Animator)), Required]
        private int LeftRightVelocityAnimatorParameter { get; set; }
        
        [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
        [field: ShowIf(nameof(this.Animator)), Required]
        private int ForwardBackVelocityAnimatorParameter { get; set; }
        
        [field: SerializeField, MinValue(0)] private float AnimationBlendTime { get; set; } = 0.1f;
        private double Speed { get; set; } = 1;
        
        private AdvancedDropdownList<string> AllAttributes => AttributeUtils.GetDropdownList();

        private void Awake() {
            this.Locomotion = this.GetComponent<Locomotion>();
        }

        private void OnEnable() {
            this.AttributeSet = this.Locomotion.Root.GetOrAdd<AttributeSet>();
            this.AttributeSet.OnAttributeUpdated += this.OnAttributeUpdated;
        }

        private void OnAttributeUpdated(AttributeChange change) {
            if (change.Attribute == this.MovementSpeedAttribute) {
                this.Speed = change.NewValue;
            }
        }

        private void Update() {
            Vector2 input = Singleton<PlayerInputInterpreter>.Instance.MovementInput * (float)this.Speed;
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