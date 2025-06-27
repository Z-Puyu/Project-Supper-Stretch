using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.Characters;

[DisallowMultipleComponent]
public class Ragdoll : MonoBehaviour {
    private List<CharacterJoint> Joints { get; set; } = [];
    private List<Rigidbody> BodyParts { get; init; } = [];
    private List<Collider> Colliders { get; init; } = [];
    [NotNull] [field: SerializeField] private Animator? Animator { get; set; }

    private void Awake() {
        this.GetComponentsInChildren(this.Joints);
        this.Joints.ForEach(register);
        this.BodyParts.ForEach(body => body.isKinematic = true);
        this.Colliders.ForEach(c => c.enabled = false);
        if (!this.Animator) {
            this.Animator = this.GetComponentInChildren<Animator>();
        }
        
        if (!this.Animator) {
            Debug.LogError($"{this.GetType().Name} requires an Animator to function!", this);
        }
        
        return;
        
        void register(CharacterJoint joint) {
            this.BodyParts.Add(joint.GetComponent<Rigidbody>());
            this.Colliders.Add(joint.GetComponent<Collider>());
        }
    }

    private void OnEnable() {
        this.BodyParts.ForEach(body => body.isKinematic = false);
        this.Colliders.ForEach(c => c.enabled = true);
        this.Animator.enabled = false;
    }
    
    private void OnDisable() {
        this.BodyParts.ForEach(body => body.isKinematic = true);
        this.Colliders.ForEach(c => c.enabled = false);
        this.Animator.enabled = true;
    }
}
