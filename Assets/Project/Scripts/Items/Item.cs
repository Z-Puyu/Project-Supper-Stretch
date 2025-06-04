using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Items.Equipments;
using Project.Scripts.Util.Linq;
using Project.Scripts.Util.Visitor;
using UnityEngine;

namespace Project.Scripts.Items;

public abstract class Item : ScriptableObject, IVisitor<AttributeSet>, IVisitor<EquipmentSystem> {
    [field: SerializeField]
    public string Name { get; private set; } = string.Empty;
    
    [field: SerializeField]
    public GameObject? Model { get; private set; }
    
    public abstract ItemProperty Properties { get; }

    public abstract IEnumerable<Modifier> EffectsWhenUsedOn(AttributeSet target);
    
    public void Visit(AttributeSet attributes) {
        GameplayEffectFactory.CreateInstant<UseItem>().Execute(new ItemUsageExecutionArgs(attributes, this));
    }

    public abstract void Visit(EquipmentSystem visitable);
}
