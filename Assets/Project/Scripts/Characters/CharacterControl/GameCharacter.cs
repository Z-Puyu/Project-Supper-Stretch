using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.Characters.CharacterControl.Combat;
using Project.Scripts.Common;
using Project.Scripts.Items.Equipments;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.Util.Linq;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Project.Scripts.Characters.CharacterControl;

[DisallowMultipleComponent, RequireComponent(typeof(AttributeSet), typeof(Rigidbody))]
[RequireComponent(typeof(Inventory), typeof(ComboAttack), typeof(Ragdoll))]
public abstract class GameCharacter : MonoBehaviour {
    [field: SerializeField] private List<GameObject> DestroyOnDeath { get; set; } = [];
    [field: SerializeField] private List<GameObject> EnableAfterDeath { get; set; } = [];
    [field: SerializeField, Required] private GameObject? CharacterModel { get; set; }
    
    [NotNull] 
    [field: SerializeField, Required] 
    protected Animator? Animator { get; private set; }
    
    [field: SerializeField, HideIf(nameof(this.Animator), null)]
    [field: AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
    private int DeathAnimationTrigger { get; set; }
    
    // [NotNull] private Health? Health { get; set; }
    [NotNull] protected Inventory? Inventory { get; private set; }
    [NotNull] protected ComboAttack? ComboAttack { get; private set; }

    protected virtual void Awake() {
        this.Inventory = this.GetComponent<Inventory>();
        this.ComboAttack = this.GetComponent<ComboAttack>();
        this.GetComponent<Ragdoll>().enabled = false;
        this.GetComponent<Health>().enabled = false;
        if (!this.Animator) {
            this.Animator = this.GetComponentInChildren<Animator>();
        }

        if (!this.Animator) {
            Debug.LogError($"{this.name} requires an Animator to function!", this);
        }
    }

    protected virtual void Start() {
        this.EnableAfterDeath.ForEach(obj => obj.SetActive(false));
        this.GetComponent<Health>().OnDeath += this.DyingFrom;
        this.GetComponent<Health>().enabled = true;
    }

    protected virtual void DyingFrom(GameObject? source) {
        Debug.Log($"{this.gameObject.name} killed by {(source ? source.name : "unknown source")}", this);
        this.Animator.SetTrigger(this.DeathAnimationTrigger);
        this.GetComponents<MonoBehaviour>().ForEach(component => component.enabled = false);
        this.GetComponent<Ragdoll>().enabled = true;
        this.DestroyOnDeath.ForEach(Object.Destroy);
        this.EnableAfterDeath.ForEach(obj => obj.SetActive(true));
    }

    public void Die() {
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

    protected override void Start() {
        base.Start();
        if (!this.CharacterData) {
            Logging.Error($"{this.name} has no character data!", this);
            return;
        }
        
        this.GetComponent<AttributeSet>().Initialise(this.CharacterData.Attributes);
        this.GetComponentsInChildren<AttributeBasedHealth>().ForEach(health => health.Initialise());
        this.transform.SetParent(null);
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
