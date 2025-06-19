using System.Collections.Generic;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;
using SaintsField;
using Project.Scripts.Items.Equipments;
using UnityEngine;

namespace Project.Scripts.Items;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class ItemData : ScriptableObject {
    [field: SerializeField] public ItemType Type { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.Type), ItemType.Equipment)]
    public EquipmentSlot Slot { get; private set; } = EquipmentSlot.None;
    
    [field: SerializeField] public string Name { get; private set; } = string.Empty;
    [field: SerializeField] public GameObject? Model { get; protected set; }
    [field: SerializeField, MinValue(0)] public int Worth { get; private set; } = 1;
    [field: SerializeField] public List<Modifier> Properties { get; private set; } = [];
    [field: SerializeField] public List<GameplayEffect> Effects { get; private set; } = [];
}