using System;
using Project.Scripts.Common;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.Items.Equipments;

public class EquipmentSocket : MonoBehaviour, IComparable<EquipmentSocket> {
    [field: SerializeField] public EquipmentSlot Slot { get; private set; }
    public Item? EquippedItem { get; private set; }
    public GameObject? Equipment { get; private set; }
    
    public bool IsAvailable => !this.Equipment;

    public void Attach(in Item item, in GameObject? equipment) {
        if (!this.IsAvailable) {
            Logging.Error($"Socket {this.name} is not available.", this);
            return;
        }
        
        if (!equipment) {
            this.Equipment = new GameObject("Placeholder Equipment Model");
        }
        
        this.EquippedItem = item;
        this.Equipment = Object.Instantiate(equipment, this.transform);
    }
    
    public void Detach() {
        if (this.IsAvailable) {
            return;
        }
        
        Object.Destroy(this.Equipment);
        this.Equipment = null;
    }

    public int CompareTo(EquipmentSocket other) {
        return this.Slot.CompareTo(other.Slot);
    }

    public override string ToString() {
        return $"{this.gameObject.name} ({this.Slot})";
    }
}
