using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Util.Linq;
using SaintsField;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Scripts.Characters.Combat;

[DisallowMultipleComponent, RequireComponent(typeof(Combatant))]
public sealed class CombatKnockBack : MonoBehaviour, IKnockBackable {
    [NotNull]
    [field: SerializeField, Required]
    private Transform? RootTransform { get; set; }

    [NotNull]
    [field: SerializeField, Required]
    private Rigidbody? Rigidbody { get; set; }

    [NotNull]
    [field: SerializeField, Required]
    private NavMeshAgent? NavMeshAgent { get; set; }
    
    [NotNull]
    [field: SerializeField, Required]
    private Animator? Animator { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Float)]
    private int KnockbackStrengthParameter { get; set; }

    private void Awake() {
        this.RootTransform.GetComponentsInChildren<HitBox>().ForEach(hitbox => hitbox.OnHit += this.ApplyKnockBack);
    }

    private void Start() {
        this.Rigidbody.useGravity = false;
        this.Rigidbody.isKinematic = true;
    }

    public void ApplyKnockBack(float magnitude) {
        Vector3 force = -this.RootTransform.forward * magnitude;
        this.GetKnockedBack(force);
    }

    private void ApplyKnockBack(Damage damage, GameObject? source, HitBoxTag where) {
        Vector3 origin = !source ? damage.Origin : source.transform.root.position;
        Vector3 direction = (damage.HitPoint - origin).normalized with { y = 0 };
        Vector3 force = direction * (damage.KnockBackStrength * damage.Multiplier / 100.0f);
        this.GetKnockedBack(force);
    }

    public void GetKnockedBack(Vector3 force) {
        this.Animator.SetFloat(this.KnockbackStrengthParameter, force.magnitude);
        this.StartCoroutine(this.ApplyKnockBack(force));
    }

    private IEnumerator ApplyKnockBack(Vector3 force) {
        yield return new WaitForEndOfFrame();
        if (this.NavMeshAgent) {
            this.NavMeshAgent.enabled = false;
        }
        
        this.Rigidbody.isKinematic = false;
        this.Rigidbody.AddForce(force);
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.1f);
        this.Rigidbody.isKinematic = true;
        if (this.NavMeshAgent) {
            this.NavMeshAgent.Warp(this.RootTransform.position);
            this.NavMeshAgent.enabled = true;
        }

        yield return new WaitForEndOfFrame();
    }
}
