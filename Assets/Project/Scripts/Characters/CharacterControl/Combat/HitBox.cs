using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.CharacterControl.Combat;

[RequireComponent(typeof(BoxCollider))]
public class HitBox : MonoBehaviour {
    [field: SerializeField] private HitBoxTag Tag { get; set; }
    private bool HasTarget { get; set; }
    [NotNull] private GameObject? Owner { get; set; }
    private GameObject? CurrentTarget { get; set; }
    
    public event UnityAction<IEnumerable<IDamageable>> OnHit = delegate { };

    private void Awake() {
        this.Owner = this.transform.root.gameObject;
    }

    private void PerformHit() {
        if (!this.CurrentTarget) {
            return;
        }
        
        this.OnHit.Invoke(this.CurrentTarget.GetComponents<IDamageable>());
    }

    private bool IsInvalidHit(GameObject target) {
        return target == this.Owner || target.CompareTag(this.Owner.tag) ||
               target.transform.IsChildOf(this.Owner.transform) ||
               this.Owner.transform.IsChildOf(target.transform);
    }

    private void OnTriggerEnter(Collider other) {
        GameObject target = other.transform.root.gameObject;
        if (this.IsInvalidHit(target)) {
            return; // Because you cannot hit yourself or your teammates.
        }
        
        this.CurrentTarget = target;
        this.HasTarget = true;
    }

    private void OnTriggerExit(Collider other) {
        GameObject target = other.transform.root.gameObject;
        if (this.IsInvalidHit(target)) {
            return; // Because you cannot hit yourself or your teammates.
        }
        
        this.HasTarget = false;
        this.CurrentTarget = null;
    }

    public void RegisterAttackEvent(HitBoxTag hitbox) {
        if (hitbox != this.Tag) {
            return;
        }
        
        if (this.HasTarget) {
            this.PerformHit();
        }
    }

    public override string ToString() {
        return $"{this.Owner}'s {this.Tag} hitbox";
    }
}