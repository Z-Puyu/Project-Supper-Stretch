using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.Characters.Combat;
using Project.Scripts.Common;
using Project.Scripts.Util.Linq;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events; 
using Object = UnityEngine.Object;

namespace Project.Scripts.Characters;

[DisallowMultipleComponent, RequireComponent(typeof(Rigidbody))]
public abstract class GameCharacter : MonoBehaviour {
    [field: SerializeField, Required] private GameObject? CharacterModel { get; set; }
    
    [NotNull] 
    [field: SerializeField, Required] 
    protected Animator? Animator { get; private set; }
    
    [field: SerializeField, HideIf(nameof(this.Animator), null)]
    [field: AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Bool)]
    private int DeathAnimationParameter { get; set; }
    
    [field: SerializeField, Required] public Health? HealthComponent { get; private set; }
    
    public event UnityAction? OnKilled;

    protected virtual void Awake() {
        if (!this.Animator) {
            this.Animator = this.GetComponentInChildren<Animator>();
        }

        if (!this.Animator) {
            Logging.Error($"{this.name} requires an Animator to function!", this);
        } 
    }

    protected virtual void Start() {
        GameEvents.OnPause += this.OnPause;
        GameEvents.OnPlay += this.OnPlay;
        if (!this.HealthComponent) {
            Logging.Error($"{this.name} has no health component!", this);
            return;
        }

        this.HealthComponent.Initialise();
        this.HealthComponent.OnDeath += this.DyingFrom;
        this.OnPlay();
    }
    
    protected virtual void DyingFrom(GameObject? source) {
        Logging.Info($"{this.gameObject.name} killed by {(source ? source.name : "unknown source")}", this);
        this.Animator.SetBool(this.DeathAnimationParameter, true);
        GameEvents.OnPause -= this.OnPause;
        GameEvents.OnPlay -= this.OnPlay;
    }

    [Button("Debug: Kill")]
    public virtual void Kill() {
        this.GetComponents<Component>().Where(c => c.GetType() != typeof(Transform)).ForEach(Object.Destroy);
        this.Animator.enabled = false;
        this.Animator.GetComponents<Component>()
            .Where(c => c.GetType() != typeof(Transform))
            .ForEach(Object.Destroy);
        
        this.OnKilled?.Invoke();
    }

    protected virtual void OnPause() { }

    protected virtual void OnPlay() { }

    protected virtual void OnDestroy() {
        GameEvents.OnPause -= this.OnPause;
        GameEvents.OnPlay -= this.OnPlay;
        this.OnKilled = null;
    }
}

public abstract class GameCharacter<C> : GameCharacter where C : CharacterData {
    public static event UnityAction<C, GameObject?> OnDeath = delegate { }; 
    
    [NotNull] 
    [field: SerializeField, Required] 
    protected C? CharacterData { get; set; }
    
    [NotNull] 
    [field: SerializeField, Required]
    protected AttributeSet? AttributeSet { get; set; }

    protected override void Start() {
        if (!this.CharacterData) {
            Logging.Error($"{this.name} has no character data!", this);
            return;
        }
        
        this.AttributeSet.Initialise(this.CharacterData.Attributes);
        base.Start();
    }

    protected override void DyingFrom(GameObject? source) {
        GameCharacter<C>.OnDeath.Invoke(this.CharacterData, source);
        base.DyingFrom(source);
    }
}
