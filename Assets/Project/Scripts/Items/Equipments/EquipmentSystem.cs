using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.Util.Visitor;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Items.Equipments;

public class EquipmentSystem : MonoBehaviour, IVisitable<EquipmentSystem> {
    private Dictionary<Equipment, EquipmentSocket> EquipmentLookup { get; init; } = [];
    
    [field: SerializeField]
    private List<EquipmentSocket> EquipmentSockets { get; set; } = [];
    
    public event UnityAction<Equipment> OnEquip = delegate { };
    public event UnityAction<Equipment> OnUnequip = delegate { };

    private void Awake() {
        this.GetComponentsInChildren(this.EquipmentSockets);
    }

    private void Start() {
        this.EquipmentSockets.Sort();
    }

    /// <summary>
    /// Finds the first socket that fits the given equipment.
    /// </summary>
    /// <param name="equipment">The equipment.</param>
    /// <param name="socket">The first empty socket. If none of the sockets is empty,
    /// this is the first socket with a suitable slot.</param>
    /// <returns>True if an empty suitable socket is found.</returns>
    /// <exception cref="ArgumentException">Thrown when none of the sockets has a matching equipment slot.</exception>
    private bool FindSlot(Equipment equipment, out EquipmentSocket socket) {
        EquipmentSocket[] sockets = this.EquipmentSockets.Where(socket => socket.Fits(equipment)).ToArray();
        if (!sockets.Any()) {
            throw new ArgumentException($"No socket fits {equipment}");
        }
        
        EquipmentSocket? s = sockets.FirstOrDefault(socket => socket.IsAvailable);
        if (!s) {
            socket = sockets[0];
            return false;
        }
        
        socket = s;
        return true;
    }

    public void ToggleEquipment(Equipment equipment) {
        if (this.EquipmentLookup.ContainsKey(equipment)) {
            this.Unequip(equipment);
        } else {
            this.Equip(equipment);
        }
    }

    private void Equip(Equipment equipment) {
        if (!this.FindSlot(equipment, out EquipmentSocket socket)) {
            socket.Detach();
        }
        
        Debug.Log($"{this.gameObject} equips {equipment} to {socket}");
        socket.Attach(equipment.Model);
        this.EquipmentLookup[equipment] = socket;
        this.OnEquip.Invoke(equipment);
    }

    private void Unequip(Equipment equipment) {
        Debug.Log($"{this.gameObject} unequips {equipment} from {this.EquipmentLookup[equipment]}");
        this.EquipmentLookup[equipment].Detach();
        this.EquipmentLookup.Remove(equipment);
        this.OnUnequip.Invoke(equipment);
    }

    public void Accept(IVisitor<EquipmentSystem> visitor) {
        visitor.Visit(this);
    }
}