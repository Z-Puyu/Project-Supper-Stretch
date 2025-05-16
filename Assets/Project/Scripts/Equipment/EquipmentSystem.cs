using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem;
using Project.Scripts.Equipment.Events;
using Project.Scripts.Events;
using UnityEngine;

namespace Project.Scripts.Equipment;

[RequireComponent(typeof(AttributeSet))]
public class EquipmentSystem : MonoBehaviour {
    [field: SerializeField]
    private List<EquipmentSocket> EquipmentSockets { get; set; } = [];
    
    [field: SerializeField]
    private Equipment? TestWeapon { set; get; }
    
    [field: SerializeField]
    private EventChannel<EquipmentChangeArgs>? OnEquipped { get; set; }
    
    [field: SerializeField]
    private EventChannel<EquipmentChangeArgs>? OnUnequipped { get; set; }

    private void Start() {
        this.EquipmentSockets.Sort();
        if (this.TestWeapon != null) {
            this.Equip(this.TestWeapon);
        }
    }

    public void Equip(Equipment equipment) {
        EquipmentSocket[] sockets = this.EquipmentSockets.Where(socket => socket.Fits(equipment)).ToArray();
        if (!sockets.Any()) {
            throw new ArgumentException($"No socket fits {equipment}");
        }
        
        EquipmentSocket? socket = sockets.FirstOrDefault(socket => socket.IsAvailable);
        if (socket == null) {
            socket = sockets[0];
            this.Unequip(socket);
        }

        socket.Attach(equipment.Model ?? new GameObject("Placeholder Equipment Model"));
        this.OnEquipped?.Broadcast(this, new EquipmentChangeArgs(equipment, socket, true));
    }

    public Equipment Unequip(EquipmentSocket socket) {
        Equipment? equipment = socket.Detach();
        if (equipment == null) {
            throw new ArgumentException($"Cannot unequip from {socket}");
        }

        this.OnUnequipped?.Broadcast(this, new EquipmentChangeArgs(equipment, socket, false));
        return equipment;
    }
}