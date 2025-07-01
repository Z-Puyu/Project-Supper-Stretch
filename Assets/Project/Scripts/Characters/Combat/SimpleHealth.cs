using Project.Scripts.Common;
using UnityEngine;

namespace Project.Scripts.Characters.Combat;

public class SimpleHealth : Health {
    protected override bool IsAttributeBased => false;

    protected override void TakeDamage(Damage damage, HitBoxTag where = HitBoxTag.Generic) {
        base.TakeDamage(damage, where);
        if (damage.Effect) {
            Logging.Warn("The damage has a gameplay effect but the target's health is not attribute-based", this);
        }
            
        this.UpdateHealth(this.Current - damage.Multiplier);
    }
    
    public override void Heal(int amount, GameObject? source) { }

    public override string ToString() {
        return $"Health: {this.Current} / {this.Max}";
    }
}
