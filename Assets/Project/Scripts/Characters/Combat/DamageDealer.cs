﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.Characters.Player;
using Project.Scripts.Common;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.Combat;

[DisallowMultipleComponent]
public sealed class DamageDealer : MonoBehaviour {
    private AdvancedDropdownList<string> AllAttributes => GameplayTagTree<AttributeType>.Instances
                                                                                        .OfType<AttributeDefinition>()
                                                                                        .LeafTags();
    
    [NotNull] [field: SerializeField] private GameObject? Owner { get; set; }
    private HitBox? CurrentTarget { get; set; }
    private bool HasTarget { get; set; }

    [field: SerializeField, Tag]
    [field: Tooltip("Tags ignored by the damage dealer. The root object's tag is always ignored regardless.")]
    private List<string> FriendlyTags { get; set; } = [];

    [NotNull]
    [field: SerializeField, Required]
    private Collider? TargetDetector { get; set; }

    [field: SerializeField, AdvancedDropdown(nameof(this.AllAttributes))]
    [field: InfoBox("Which attribute's value to use as a reference for damage calculation?")]
    private string BaseDamageAttribute { get; set; } = string.Empty;

    [field: InfoBox("Damage calculation requires an attribute reader!", EMessageType.Error, nameof(this.IsInvalid))]
    private IAttributeReader? AttributeReader { get; set; }
    
    private Vector3 StartPosition { get; set; }
    public float KnockbackStrength { get; set; }
    
    private bool IsInvalid => this.GetComponentInParent<IAttributeReader>() == null;
    
    public event UnityAction? OnAttack;
    public event UnityAction? OnBlocked;
    public event UnityAction? OnKnockedBack;

    private void Awake() {
        if (!this.Owner) {
            this.Owner = this.transform.root.gameObject;
        }
        
        this.AttributeReader = this.GetComponent<IAttributeReader>();
        if (this.AttributeReader == null) {
            Logging.Error($"{this.name} requires an {nameof(IAttributeReader)} to function!", this);
        }
    }

    private void Start() {
        this.TargetDetector.enabled = false;
    }

    private void OnDestroy() {
        this.OnAttack = null;
        this.OnBlocked = null;
        this.OnKnockedBack = null;
    }

    public void Enable() {
        this.OnAttack?.Invoke();
        this.TargetDetector.enabled = true;
    }

    public void TryPerformHit() {
        this.TargetDetector.enabled = false;
        if (!this.HasTarget) { 
            return;
        }

        // The attacker chooses what kind of damage to inflict.
        Vector3 hitPoint = this.transform.position;
        Damage dmg = new Damage(this.StartPosition, hitPoint, this.KnockbackStrength);
        this.CurrentTarget!.TakeDamage(dmg, this.gameObject);
        this.CurrentTarget = null;
        this.HasTarget = false;
        this.KnockbackStrength = 0;
    }

    public void Blocked(bool isBlockedByParry) {
        if (isBlockedByParry) {
            this.OnKnockedBack?.Invoke();
        } else {
            this.OnBlocked?.Invoke();
        }
        
        Logging.Info($"Damage by {this.transform.root.name} blocked!", this);
    }

    private bool IsValidHit(GameObject target, Collider at, out HitBox? hitPoint) {
        bool isFriendlyTarget = target == this.Owner || target.CompareTag(this.Owner.tag) ||
                                this.FriendlyTags.Contains(target.tag) ||
                                target.transform.IsChildOf(this.Owner.transform) ||
                                this.Owner.transform.IsChildOf(target.transform);
        if (!isFriendlyTarget && at.TryGetComponent(out hitPoint)) {
            return true;
        }

        hitPoint = null;
        return false;
    }

    private void OnTriggerEnter(Collider other) {
        GameObject target = other.transform.root.gameObject;
        if (!this.IsValidHit(target, other, out HitBox? hitPoint)) {
            return;
        }

        this.StartPosition = this.transform.position;
        this.CurrentTarget = hitPoint;
        this.HasTarget = true;
    }
}
