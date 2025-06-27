using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Common;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Items.Equipments;

[DisallowMultipleComponent]
public class EquipmentSystem : MonoBehaviour {
    private Dictionary<Item, EquipmentSocket> EquipmentLookup { get; init; } = [];
    private List<EquipmentSocket> EquipmentSockets { get; set; } = [];
    
    [field: SerializeField] private Animator? Animator { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Bool)]
    private int DrawWeaponCondition { get; set; }

    private void Awake() {
        this.GetComponentsInChildren(this.EquipmentSockets);
    }

    private void Start() {
        this.EquipmentSockets.Sort();
    }

    public bool Equip(Item equipment, GameObject model, EquipmentSlot slot, out EquipmentSocket socket) {
        EquipmentSocket[] sockets = this.EquipmentSockets.Where(socket => slot.HasFlag(socket.Slot)).ToArray();
        if (!sockets.Any()) {
            Logging.Error($"{this.gameObject.name} has no socket with slot {slot}", this);
            socket = this.EquipmentSockets[0];
            return false;       
        }
        
        EquipmentSocket? available = sockets.FirstOrDefault(socket => socket.IsAvailable);
        if (!available) {
            socket = sockets[0];
            return false;       
        }
        
        available.Attach(equipment, model);
        this.EquipmentLookup[equipment] = available;
        socket = available;
        if (equipment.Type.HasFlag(ItemFlag.Weapon) && this.Animator) {
            this.Animator.SetBool(this.DrawWeaponCondition, true);
        }
        
        return true;
    }

    public bool Unequip(in Item equipment) {
        if (!this.EquipmentLookup.TryGetValue(equipment, out EquipmentSocket socket)) {
            return false;
        }
        
        socket.Detach();
        this.EquipmentLookup.Remove(equipment);
        if (equipment.Type.HasFlag(ItemFlag.Weapon) && this.Animator) {
            this.Animator.SetBool(this.DrawWeaponCondition, false);
        }
        
        return true;
    }
}