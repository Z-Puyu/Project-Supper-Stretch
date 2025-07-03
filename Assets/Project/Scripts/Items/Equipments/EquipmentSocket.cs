using System;
using Project.Scripts.Common;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.Items.Equipments;

public class EquipmentSocket : MonoBehaviour, IComparable<EquipmentSocket> {
    [field: SerializeField] public EquipmentSlot Slot { get; private set; }
    public Item? EquippedItem { get; private set; }
    private GameObject? Equipment { get; set; }
    
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
        if (this.IsAvailable || this.EquippedItem is null) {
            return;
        }
        
        Object.Destroy(this.Equipment);
        this.Equipment = null;
    }

    public bool Holds<T>(Predicate<Item>? predicate = null) where T : Component {
        if (this.Equipment && this.Equipment.TryGetComponent(out T _)) {
            return this.EquippedItem is not null && (predicate?.Invoke(this.EquippedItem) ?? true);
        }

        return false;
    }
    
    public bool Holds<T>(out T? component, Predicate<Item>? predicate = null) where T : Component {
        if (this.Equipment && this.Equipment.TryGetComponent(out component)) {
            return this.EquippedItem is not null && (predicate?.Invoke(this.EquippedItem) ?? true);
        }
        
        component = null;
        return false;
    }

    public int CompareTo(EquipmentSocket other) {
        return this.Slot.CompareTo(other.Slot);
    }

    public override string ToString() {
        return $"{this.gameObject.name} ({this.Slot})";
    }
}
