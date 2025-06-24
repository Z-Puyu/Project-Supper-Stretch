using System;

namespace Project.Scripts.Items;

[Flags]
public enum ItemFlag {
    Consumable = 1 << 0,
    Equipable = 1 << 1,
    CraftingMaterial = 1 << 2,
    Currency = 1 << 3,
    Miscellaneous = 1 << 4
}
