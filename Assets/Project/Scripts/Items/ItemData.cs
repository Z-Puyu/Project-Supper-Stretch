using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Editor;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common;
using Project.Scripts.Common.GameplayTags;
using Project.Scripts.Items.Definitions;
using SaintsField;
using Project.Scripts.Items.Equipments;
using UnityEngine;

namespace Project.Scripts.Items;

[CreateAssetMenu(fileName = "Item Data", menuName = "Item/Data")]
public class ItemData : ScriptableObject {
    [NotNull] 
    [field: SerializeField, AdvancedDropdown(nameof(this.AllItemTypes))]
    public ItemType? Type { get; private set; }

    [field: SerializeField] public string Name { get; private set; } = string.Empty;
    [field: SerializeField] public GameObject? Model { get; protected set; }
    [field: SerializeField, MinValue(0)] public int Worth { get; private set; } = 1;
    [field: SerializeField] public List<Modifier> Properties { get; private set; } = [];
    
    [field: SerializeField, ShowIf(nameof(this.IsCraftingMaterial))]
    public float CraftTime { get; private set; } = 0.2f;
    
    [field: SerializeField, ShowIf(nameof(this.IsEquipment))]
    public EquipmentSlot Slot { get; private set; } = EquipmentSlot.None;

    private bool IsEquipment() {
        return this.Type.HasFlag(ItemFlag.Equipable);
    }

    private bool IsCraftingMaterial() {
        return this.Type.HasFlag(ItemFlag.CraftingMaterial);
    }

    private bool IsConsumable() {
        return this.Type.HasFlag(ItemFlag.Consumable);
    }

    private AdvancedDropdownList<ItemType> AllItemTypes => ObjectCache<ItemDefinition>.Instance.Objects.AllNodes();
}