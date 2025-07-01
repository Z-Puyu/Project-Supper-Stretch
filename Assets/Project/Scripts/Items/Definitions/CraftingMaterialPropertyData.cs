using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Modifiers;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.Definitions;

[Serializable]
public class CraftingMaterialPropertyData : IItemPropertyData {
    [field: SerializeField] public List<ModifierData> Modifiers { get; private set; } = [];
    [field: SerializeField, MinValue(0)] public float Cost { get; private set; }
    
    public ItemProperty Create() {
        return new CraftingMaterialProperty(this.Modifiers.GetModifiers().ToArray(), this.Cost);       
    }
}
