using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Editor;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.Characters.CharacterControl;
using Project.Scripts.Characters.CharacterControl.Combat;
using Project.Scripts.Common;
using Project.Scripts.Common.GameplayTags;
using Project.Scripts.Util.Linq;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters;

[DisallowMultipleComponent]
public class DamageDealer : MonoBehaviour {
    private AdvancedDropdownList<string> AllAttributes => ObjectCache<AttributeDefinition>.Instance.Objects.LeafTags();

    [NotNull] private GameObject? Owner { get; set; }
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

    [NotNull]
    [field: SerializeField, Required]
    private GameplayEffect? DamageEffect { get; set; }

    private bool IsInvalid => this.GetComponentInParent<IAttributeReader>() == null; 

    private void Awake() {
        this.Owner = this.transform.root.gameObject;
        this.AttributeReader = this.GetComponent<IAttributeReader>();
        if (this.AttributeReader == null) {
            Logging.Error($"{this.name} requires an {nameof(IAttributeReader)} to function!", this);
        }
    }

    private void OnEnable() {
        this.TargetDetector.enabled = true;
    }

    private void OnDisable() {
        this.TargetDetector.enabled = false;
        this.HasTarget = false;
        this.CurrentTarget = null;
    }

    public void TryPerformHit() {
        if (!this.HasTarget) {
            this.enabled = false;
            return;
        }
        
        // The attacker chooses what kind of damage to inflict.
        this.CurrentTarget!.TakeDamage(new Damage(this.DamageEffect, this.gameObject));
    }

    private bool IsInvalidHit(GameObject target, out HitBox? hitPoint) {
        bool isValid = target == this.Owner || target.CompareTag(this.Owner.tag) ||
                       this.FriendlyTags.Contains(target.tag) || target.transform.IsChildOf(this.Owner.transform) ||
                       this.Owner.transform.IsChildOf(target.transform);
        if (isValid && target.TryGetComponent(out hitPoint)) {
            return true;
        }

        hitPoint = null;
        return false;
    }

    private void OnTriggerEnter(Collider other) {
        GameObject target = other.transform.root.gameObject;
        if (!this.IsInvalidHit(target, out HitBox? hitPoint)) {
            return;
        }

        this.CurrentTarget = hitPoint;
        this.HasTarget = true;
    }
}
