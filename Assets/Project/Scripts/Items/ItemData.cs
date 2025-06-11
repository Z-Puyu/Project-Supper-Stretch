using System.Collections.Generic;
using Project.Scripts.Items.Equipments;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items;

public abstract class ItemData : ScriptableObject {
    [field: SerializeField]
    public ItemType Type { get; private set; }
    
    [field: SerializeField, ShowIf(nameof(this.IsEquipment))]
    public EquipmentSlot Slot { get; private set; } = EquipmentSlot.None;
    
    [field: SerializeField]
    public string Name { get; private set; } = string.Empty;
    
    [field: SerializeField]
    public GameObject? Model { get; protected set; }

    [field: SerializeReference]
    public List<IItemProperties> Properties { get; private set; } = [];
    
    [field: SerializeField, MinValue(0)]
    public int Worth { get; private set; } = 1;
    
    private bool IsEquipment => this.Type == ItemType.Equipment;
}