using System;

namespace Project.Scripts.Items;

[Flags]
public enum ItemProperty {
    None = 0,
    Usable = 1,
    Consumable = 2,
    Equipable = 4
}
