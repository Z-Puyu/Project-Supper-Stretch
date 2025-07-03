using System;

namespace Project.Scripts.Items.Equipments;

[Flags]
public enum EquipmentSlot {
    None = 0,
    RightHand = 1 << 0, 
    LeftHand = 1 << 1,
    Generic = 1 << 2
}
