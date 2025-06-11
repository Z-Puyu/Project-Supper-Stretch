using System.Collections.Generic;
using Project.Scripts.Items.Equipments;
using UnityEngine;

namespace Project.Scripts.Items;

public record Equipment(
    string Name,
    int Worth,
    EquipmentSlot Slot,
    IEnumerable<IItemProperties> Properties,
    GameObject? Model = null
) : Item(ItemType.Equipment, Name, Worth, Properties, Model);
