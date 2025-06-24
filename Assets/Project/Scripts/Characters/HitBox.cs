using System;
using Project.Scripts.Characters.CharacterControl;
using Project.Scripts.Characters.CharacterControl.Combat;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters;

[DisallowMultipleComponent, RequireComponent(typeof(BoxCollider))]
public class HitBox : MonoBehaviour {
    [field: SerializeField] private HitBoxTag Label { get; set; }
    
    [field: SerializeField, MinValue(-100)] 
    private float DamageMultiplier { get; set; } = 1;
    
    public event UnityAction<Damage, HitBoxTag> OnHit = delegate { }; 

    public void TakeDamage(Damage damage) {
        if (!damage.Exists) {
            return;
        }
        
        // The hit box will modify the power of the damage and pass on the effect.
        this.OnHit.Invoke(damage * this.DamageMultiplier, this.Label);
    }
}
