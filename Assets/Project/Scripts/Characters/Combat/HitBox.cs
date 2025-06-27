using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.Combat;

[DisallowMultipleComponent, RequireComponent(typeof(BoxCollider))]
public class HitBox : MonoBehaviour {
    [field: SerializeField] private HitBoxTag Label { get; set; }
    
    [field: SerializeField, MinValue(-100)] 
    private float DamageMultiplier { get; set; } = 1;
    
    [field: SerializeField] private List<BlockingZone> BlockingZones { get; set; } = [];
    [field: SerializeField] private Animator? Animator { get; set; }
    
    [field: SerializeField, HideIf(nameof(this.Animator), null)] 
    [field: AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Trigger)]
    private int BlockAnimationTrigger { get; set; }
    
    public event UnityAction<Damage, HitBoxTag> OnHit = delegate { }; 

    public void TakeDamage(Damage damage) {
        if (!damage.Exists) {
            return;
        }

        if (this.BlockingZones.Any(zone => zone.HasBlocked(damage.Origin))) {
            if (damage.Source) {
                damage.Source.Blocked();
            }

            if (this.Animator) {
                this.Animator.SetTrigger(this.BlockAnimationTrigger);
            }
            
            return;  
        }
        
        // The hit box will modify the power of the damage and pass on the effect.
        this.OnHit.Invoke(damage * this.DamageMultiplier, this.Label);
    }
}
