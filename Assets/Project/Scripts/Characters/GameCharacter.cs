using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.Characters.Combat;
using Project.Scripts.Common;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.Util.Linq;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Project.Scripts.Characters;

[DisallowMultipleComponent, RequireComponent(typeof(AttributeSet), typeof(Rigidbody))]
[RequireComponent(typeof(Inventory), typeof(Combatant), typeof(Ragdoll))]
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
    
    [field: SerializeField, HideIf(nameof(this.Animator), null)]
    [field: AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Int)]
    private int HitFeedbackAnimationParameter { get; set; }
    
    [NotNull] protected Inventory? Inventory { get; private set; }
    [NotNull] protected Combatant? ComboAttack { get; private set; }

    private List<Behaviour> AllComponents { get; set; } = [];
    private List<Collider> AllColliders { get; set; } = [];

    protected virtual void Awake() {
        this.GetComponents<Behaviour>()
            .Where(component => component.GetType() != typeof(Ragdoll))
            .ForEach(this.AllComponents.Add);
        this.GetComponents(this.AllColliders);
        this.Inventory = this.GetComponent<Inventory>();
        this.ComboAttack = this.GetComponent<Combatant>();
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
        this.GetComponent<Health>().OnDamaged += onHitFeedback;
        return;

        void onHitFeedback(int severity) {
            if (this.Animator) {
                this.Animator.SetInteger(this.HitFeedbackAnimationParameter, severity);
            }
        }
    }

    protected virtual void DyingFrom(GameObject? source) {
        Debug.Log($"{this.gameObject.name} killed by {(source ? source.name : "unknown source")}", this);
        this.Animator.SetTrigger(this.DeathAnimationTrigger);
        this.DestroyOnDeath.ForEach(Object.Destroy);
        foreach (Transform child in this.transform) {
            if (child.TryGetComponent(out CharacterJoint _) || !child.gameObject.activeInHierarchy) {
                continue;
            }
            
            this.AllComponents.ForEach(Object.Destroy);
            this.AllColliders.ForEach(Object.Destroy);
        }
        
        this.GetComponent<Ragdoll>().enabled = true;
        this.EnableAfterDeath.ForEach(obj => obj.SetActive(true));
    }

    public void Die() {
        if (this.Animator) {
            this.Animator.enabled = false;
        }
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
    }

    protected override void DyingFrom(GameObject? source) {
        GameCharacter<C>.OnDeath.Invoke(this.CharacterData, source);
        base.DyingFrom(source);
    }
}
