using System;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common;
using Project.Scripts.Util.Linq;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Characters.CharacterControl.Combat;

public class SimpleHealth : Health {
    protected override bool IsAttributeBased => false;

    protected override void TakeDamage(Damage damage, HitBoxTag where = HitBoxTag.Generic) {
        base.TakeDamage(damage, where);
        if (damage.Effect) {
            Logging.Warn("The damage has a gameplay effect but the target's health is not attribute-based", this);
        }
            
        this.UpdateHealth(this.Current - damage.Multiplier);
    }
    
    public void Heal(int amount, GameObject? source) { }

    public override string ToString() {
        return $"Health: {this.Current} / {this.Max}";
    }
}
