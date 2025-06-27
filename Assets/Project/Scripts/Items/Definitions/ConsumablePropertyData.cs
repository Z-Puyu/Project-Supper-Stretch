using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.Definitions;

[Serializable]
public sealed class ConsumablePropertyData : IItemPropertyData {
    [field: SerializeField] public List<ModifierData> Effects { get; private set; } = [];
    
    [NotNull] 
    [field: SerializeField, Required] 
    public GameplayEffect? GameplayEffectOnUse { get; private set; }
    

    public ItemProperty Create() {
        return new ConsumableProperty(this.Effects.GetModifiers().ToArray(), this.GameplayEffectOnUse);
    }

    public IItemProperty Produce() {
        return this.Create();       
    }
}
