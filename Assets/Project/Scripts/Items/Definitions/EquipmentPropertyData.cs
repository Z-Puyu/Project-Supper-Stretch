using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Items.Equipments;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.Definitions;

[Serializable]
public class EquipmentPropertyData : IItemPropertyData {
    [field: SerializeField] public EquipmentSlot Slot { get; private set; }
    
    [NotNull]
    [field: SerializeField, Required] 
    public GameObject? Model { get; private set; }

    [field: SerializeField] public List<ModifierData> Modifiers { get; private set; } = [];
    
    [NotNull] 
    [field: SerializeField, Required] 
    public GameplayEffect? GameplayEffectOnEquip { get; private set; }
    
    [NotNull] 
    [field: SerializeField, Required] 
    public GameplayEffect? GameplayEffectOnUnequip { get; private set; }

    public ItemProperty Create() {
        return new EquipmentProperty(this.Slot, this.Model, this.Modifiers.GetModifiers().ToArray(),
            this.GameplayEffectOnEquip, this.GameplayEffectOnUnequip);
    }
}
