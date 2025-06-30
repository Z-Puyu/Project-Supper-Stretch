using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Audio;
using Project.Scripts.Characters.Combat;
using Project.Scripts.Util.Components;
using SaintsField;
using UnityEngine;
using UnityEngine.Audio;

namespace Project.Scripts.Characters.Footsteps;

[DisallowMultipleComponent, RequireComponent(typeof(Animator))]
public class Footsteps : AudioPlayer<Footsteps.Mode> {
    public enum Mode { Walk, Run }
    
    [field: SerializeField, RichLabel(nameof(this.ClipName), isCallback: true)] 
    private List<AudioSource> DecorativeSounds { get; set; } = [];
    
    [NotNull] private Animator? Animator { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)] 
    private int FootstepParam { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
    private int SpeedParam { get; set; }

    [field: SerializeField, MinValue(0)] private float RunSpeedThreshold { get; set; } = 1.5f;
    
    [field: SerializeField] private LayerMask EnvironmentMask { get; set; }

    private float LastCheckedParameter { get; set; } = 1;
    private bool IsMuted { get; set; }
    private Vector3 LastPosition { get; set; }
    private Vector3 CurrentDirection { get; set; }
    [NotNull] private Transform? FeetTransform { get; set; }

    protected override void Awake() {
        base.Awake();
        this.FeetTransform = this.transform;
        this.Animator = this.GetComponent<Animator>();
        GameCharacter? character = this.GetComponentInParent<GameCharacter>();
        if (!character || !character.HasChildComponent(out Combatant combatant)) {
            return;
        }
        
        combatant.OnAttackEnded += () => this.IsMuted = false;
        combatant.OnAttackStarted += () => this.IsMuted = true;
    }

    private void Start() {
        this.LastPosition = this.FeetTransform.position;
    }

    private void PlayFootstepSound(bool isRunning) {
        Vector3 right = this.FeetTransform.right;
        float angle = Vector3.SignedAngle(this.FeetTransform.forward, this.CurrentDirection, right);
        Vector3 up = Quaternion.AngleAxis(angle, right) * this.FeetTransform.up;
        Vector3 footPosition = this.FeetTransform.position + up * 0.01f;
        if (!Physics.Raycast(footPosition, -up, out RaycastHit hit, 0.2f, this.EnvironmentMask)) {
            return;
        }

        Surface? surface = hit.transform.GetComponentInParent<Surface>();
        if (surface && surface.SurfaceData) {
            if (isRunning) {
                this.AudioSources[(int)Mode.Run].resource = surface.SurfaceData.PullRunningSound();
            } else {
                this.AudioSources[(int)Mode.Walk].resource = surface.SurfaceData.PullWalkingSound();
            }
        }

        this.AudioSources[isRunning ? (int)Mode.Run : (int)Mode.Walk].Play();
        this.DecorativeSounds[isRunning ? (int)Mode.Run : (int)Mode.Walk].Play();
    }

    private void Update() {
        Vector3 position = this.FeetTransform.position;
        this.CurrentDirection = ((position - this.LastPosition) * 10).normalized;
        this.LastPosition = position;
        if (this.IsMuted) {
            return;
        }
        
        float speed = this.Animator.GetFloat(this.SpeedParam);
        if (speed < 0.01f) {
            return;       
        }
        
        float parameter = this.Animator.GetFloat(this.FootstepParam);
        if ((this.LastCheckedParameter >= 0 && parameter <= 0) || (this.LastCheckedParameter <= 0 && parameter >= 0)) {
            this.PlayFootstepSound(speed >= this.RunSpeedThreshold);
        }
        
        this.LastCheckedParameter = parameter;
    }

    private void OnValidate() {
        if (!this.Animator) {
            this.Animator = this.GetComponent<Animator>();
        }
    }
}
