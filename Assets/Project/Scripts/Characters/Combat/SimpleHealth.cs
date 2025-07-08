using Project.Scripts.Common;
using UnityEngine;

namespace Project.Scripts.Characters.Combat;

public class SimpleHealth : Health {
    protected override bool IsAttributeBased => false;

    protected override void TakeDamage(Damage damage, GameObject? source, HitBoxTag where = HitBoxTag.Generic) {
        base.TakeDamage(damage, source, where);
        this.UpdateHealth(this.Current - damage.Multiplier);
    }
    
    public override void Heal(int amount, GameObject? source) { }

    public override string ToString() {
        return $"Health: {this.Current} / {this.Max}";
    }
}
