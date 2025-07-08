using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.Combat;

public sealed class BlockingZone : MonoBehaviour {
    [field: SerializeField, MinMaxSlider(-180, 180)] 
    private Vector2Int BlockingAngleRange { get; set; }
    
    [field: SerializeField] private float ParryWindow { get; set; } = 0.5f;
    [field: SerializeField] private bool UseLocalCoordinates { get; set; }
    [NotNull] private Transform? Self { get; set; }
    private float ParryWindowEnd { get; set; }
    public bool CanParry { get; set; }
    private bool IsBlocking { get; set; }
    
    public event UnityAction? OnHeldUp;
    public event UnityAction? OnBlocked;
    public event UnityAction? OnParried;
    
    private void Awake() {
        this.Self = this.UseLocalCoordinates
                ? this.transform
                : this.transform.GetComponentInParent<Animator>().transform;
    }

    private void OnEnable() {
        Logging.Info($"{this.transform.root.name} is blocking!", this);
        this.OnHeldUp?.Invoke();
        this.IsBlocking = true;
        this.CanParry = true;
        this.ParryWindowEnd = Time.time + this.ParryWindow;
    }
    
    private void OnDisable() {
        Logging.Info($"{this.transform.root.name} finished blocking!", this);
        this.IsBlocking = false;
        this.CanParry = false;
    }

    private void OnDestroy() {
        this.OnHeldUp = null;
        this.OnBlocked = null;
        this.OnParried = null;   
    }

    public bool HasBlocked(Vector3 damageDirection, out bool hasParried) {
        if (!this.IsBlocking) {
            hasParried = false;
            return false;       
        }
        
        float angle = Vector3.SignedAngle(this.Self.forward, -damageDirection, this.Self.up);
        hasParried = this.CanParry;
        bool blocked = angle < this.BlockingAngleRange.y && angle > this.BlockingAngleRange.x;
        if (!blocked) {
            return blocked;
        }

        this.OnBlocked?.Invoke();
        if (hasParried) {
            this.OnParried?.Invoke();
        }

        return blocked;
    }

    private void Update() {
        if (!this.CanParry) {
            return;
        }
        
        if (Time.time >= this.ParryWindowEnd) {
            this.CanParry = false;       
        }
    }
}
