using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters;

[DisallowMultipleComponent]
public class Ragdoll : MonoBehaviour {
    private List<CharacterJoint> Joints { get; set; } = [];
    private List<Rigidbody> BodyParts { get; init; } = [];
    private List<Collider> Colliders { get; init; } = [];
    
    [NotNull] 
    [field: SerializeField, Required] 
    private GameObject? CharacterModel { get; set; }

    private void Awake() {
        this.CharacterModel.GetComponentsInChildren(this.Joints);
        this.Joints.ForEach(register);
        this.BodyParts.ForEach(body => body.isKinematic = true);
        this.Colliders.ForEach(c => c.enabled = false);
        return;
        
        void register(CharacterJoint joint) {
            this.BodyParts.Add(joint.GetComponent<Rigidbody>());
            this.Colliders.Add(joint.GetComponent<Collider>());
        }
    }

    private void OnEnable() {
        this.BodyParts.ForEach(body => body.isKinematic = false);
        this.Colliders.ForEach(c => c.enabled = true);
    }
    
    private void OnDisable() {
        this.BodyParts.ForEach(body => body.isKinematic = true);
        this.Colliders.ForEach(c => c.enabled = false);
    }
}
