using System.Collections.Generic;
using Project.Scripts.Items.Equipments;
using UnityEngine;

namespace Project.Scripts.Items;

public record Item(
    ItemType Type,
    string Name,
    int Worth,
    IEnumerable<IItemProperties> Properties,
    GameObject? Model = null,
    EquipmentSlot Slot = EquipmentSlot.None
) {
    public static Item From(ItemData data) {
        return new Item(data.Type, data.Name, data.Worth, data.Properties, data.Model, data.Slot);
    }
    
    public override string ToString() {
        return this.Name;
    } 
}
