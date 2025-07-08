using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Util.Linq;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.Combat;

[DisallowMultipleComponent]
public abstract class Health : MonoBehaviour {
    [NotNull] 
    [field: SerializeField, Required] 
    private GameObject? Root { get; set; }
    
    [field: SerializeField, ReadOnly(nameof(this.IsAttributeBased))] 
    protected int Current { get; private set; }
    
    [field: SerializeField, ReadOnly(nameof(this.IsAttributeBased))] 
    protected int Max { get; private set; }
    
    private GameObject? LastAttacker { get; set; }

    public event UnityAction? OnDamaged;
    public event UnityAction<GameObject?>? OnDeath;
    
    protected abstract bool IsAttributeBased { get; }

    protected virtual void Start() {
        this.Root.GetComponentsInChildren<HitBox>().ForEach(hitbox => hitbox.OnHit += this.TakeDamage);
    }

    protected void OnDestroy() {
        this.OnDamaged = null;
        this.OnDeath = null;
    }

    public virtual void Initialise() { }

    protected void UpdateHealth(int health) {
        int @new = this.Max >= 0 ? Mathf.Clamp(health, 0, this.Max) : health;
        this.Current = @new;
        if (this.Current <= 0) {
            this.OnDeath?.Invoke(this.LastAttacker);
        }
    }

    protected void UpdateMaxHealth(int max) {
        this.Max = max;
        if (this.Max < this.Current) {
            this.UpdateHealth(this.Max);
        }
    }

    protected virtual void TakeDamage(Damage damage, GameObject? source, HitBoxTag where = HitBoxTag.Generic) {
        this.LastAttacker = source;
        this.OnDamaged?.Invoke();
    }

    public abstract void Heal(int amount, GameObject? source);

    public override string ToString() {
        return $"Health: {this.Current} / {this.Max}";
    }
}
