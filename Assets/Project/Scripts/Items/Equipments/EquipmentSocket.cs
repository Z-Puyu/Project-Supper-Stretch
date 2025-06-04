using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.Items.Equipments;

public class EquipmentSocket : MonoBehaviour, IComparable<EquipmentSocket> {
    [field: SerializeField]
    private EquipmentSlot Slot { get; set; }
    
    private GameObject? Equipment { get; set; }
    
    public bool IsAvailable => !this.Equipment;

    public void Attach(GameObject? equipment) {
        if (!this.IsAvailable) {
            this.Detach();
        }
        
        if (!equipment) {
            equipment = new GameObject("Placeholder Equipment Model");
        }
        
        this.Equipment = Object.Instantiate(equipment, this.transform);
    }
    
    public void Detach() {
        if (this.IsAvailable) {
            return;
        }
        
        Object.Destroy(this.Equipment);
        this.Equipment = null;
    }

    public bool Fits(Equipment equipment) {
        return equipment.Slot.HasFlag(this.Slot);
    }

    public int CompareTo(EquipmentSocket other) {
        return this.Slot.CompareTo(other.Slot);
    }

    public override string ToString() {
        return $"{this.gameObject.name} ({this.Slot})";
    }
}
