using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.Characters.CharacterControl.Combat;
using Project.Scripts.Common;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.Util.Linq;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Project.Scripts.Characters.CharacterControl;

[DisallowMultipleComponent, RequireComponent(typeof(AttributeSet), typeof(Health))]
[RequireComponent(typeof(Inventory), typeof(ComboAttack))]
public abstract class GameCharacter : MonoBehaviour {
    private List<CharacterJoint> Joints { get; set; } = [];
    private List<Rigidbody> RagdollParts { get; set; } = [];
    private List<Collider> RagdollColliders { get; set; } = [];
    [field: SerializeField, Required] private GameObject? CharacterModel { get; set; }
    [field: SerializeField, Required] protected Animator? Animator { get; private set; }
    
    [field: SerializeField, HideIf(nameof(this.Animator), null)]
    [field: AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
    private int AnimatorParameterForDeathEvent { get; set; }
    
    [field: SerializeField] private List<GameObject> DestroyOnDeath { get; set; } = [];
    [field: SerializeField] private List<GameObject> EnableAfterDeath { get; set; } = [];
    [NotNull] protected Inventory? Inventory { get; private set; }
    [NotNull] protected ComboAttack? ComboAttack { get; private set; }
    [NotNull] protected Health? Health { get; private set; }
    [NotNull] protected AttributeSet? AttributeSet { get; private set; }

    protected virtual void Awake() {
        this.ComboAttack = this.GetComponent<ComboAttack>();
        this.Inventory = this.GetComponent<Inventory>();
        this.AttributeSet = this.GetComponent<AttributeSet>();
        this.Health = this.GetComponent<Health>();
        if (!this.CharacterModel) {
            return;
        }

        CharacterJoint[] joints = this.CharacterModel.GetComponentsInChildren<CharacterJoint>();
        int l = joints.Length;
        joints.ForEach(register);
        return;

        void register(CharacterJoint joint) {
            this.RagdollParts.Add(joint.GetComponent<Rigidbody>());
            this.RagdollColliders.Add(joint.GetComponent<Collider>());
        }
    }

    protected virtual void Start() {
        this.RagdollParts.ForEach(body => body.isKinematic = true);
        this.RagdollColliders.ForEach(c => c.enabled = false);
    }

    protected virtual void DyingFrom(GameObject? source) {
        if (!this.Animator) {
            this.Animator = this.GetComponentInChildren<Animator>();
        }

        if (!this.Animator) {
            return;
        }

        Debug.Log($"{this.gameObject.name} killed by {(source ? source.name : "unknown source")}");
        this.Animator.SetTrigger(this.AnimatorParameterForDeathEvent);
        this.GetComponents<MonoBehaviour>().ForEach(component => component.enabled = false);
        this.DestroyOnDeath.ForEach(Object.Destroy);
        this.EnableAfterDeath.ForEach(obj => obj.SetActive(true));
        // Object.Destroy(this.gameObject);
    }

    public void Die() {
        this.RagdollParts.ForEach(body => body.isKinematic = false);
        this.RagdollColliders.ForEach(c => c.enabled = true);
        if (this.Animator) {
            this.Animator.enabled = false;
        }
        
        Debug.Log($"{this.gameObject.name} is actually dead");
    }
}

public abstract class GameCharacter<C> : GameCharacter where C : CharacterData {
    public static event UnityAction<C, GameObject?> OnDeath = delegate { }; 
    
    [NotNull] 
    [field: SerializeField, Required] 
    protected C? CharacterData { get; set; }
    
    [field: SerializeField] private List<GameObject> EnableAfterInitialisation { get; set; } = [];
    private bool IsInitialised { get; set; }

    public virtual void Initialise() {
        if (this.IsInitialised) {
            throw new InvalidOperationException($"Trying to initialise {this.gameObject.name} twice");
        }
        
        this.IsInitialised = true;
        this.InitialiseAttributes();
        this.InitialiseHealth();
        this.EnableAfterInitialisation.ForEach(obj => obj.SetActive(true));
        this.transform.SetParent(null);
    }
    
    private void InitialiseHealth() {
        this.Health.Initialise();
        this.Health.OnDeath += this.DyingFrom;
    }

    private void InitialiseAttributes() {
        if (!this.CharacterData) {
            return;
        }
        
        this.AttributeSet.Initialise(this.CharacterData.Attributes);
    }

    protected override void DyingFrom(GameObject? source) {
        GameCharacter<C>.OnDeath.Invoke(this.CharacterData, source);
        base.DyingFrom(source);
    }
    
    #region Debug

    [Button("Debug: Kill")]
    private void DebugKill() {
        this.DyingFrom(null);
    }
    
    #endregion
}
