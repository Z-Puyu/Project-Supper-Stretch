using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.Equipment;

public class EquipmentSocket : MonoBehaviour, IComparable<EquipmentSocket> {
    private enum Tag {
        RightHand = EquipmentSlot.RightHand
    }
    
    [field: SerializeField]
    private Tag Slot { get; set; }
    
    private GameObject? Equipment { get; set; }
    
    public bool IsAvailable => this.Equipment == null;

    public void Attach(GameObject equipment) {
        if (this.Equipment != null) {
            Object.Destroy(this.Equipment);
        }

        this.Equipment = Object.Instantiate(equipment, this.transform);
    }
    
    public Equipment? Detach() {
        if (this.Equipment == null) {
            return null;
        }

        Equipment equipment = this.Equipment.GetComponent<Equipment>();
        Object.Destroy(this.Equipment);
        this.Equipment = null;
        return equipment;
    }

    public bool Fits(Equipment equipment) {
        return equipment.Slot.HasFlag(this.Slot);
    }

    public int CompareTo(EquipmentSocket other) {
        return this.Slot.CompareTo(other.Slot);
    }
}
