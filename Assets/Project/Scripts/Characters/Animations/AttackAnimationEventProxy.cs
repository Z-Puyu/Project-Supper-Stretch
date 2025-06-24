using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Characters.CharacterControl.Combat;
using Project.Scripts.Characters.Player;
using Project.Scripts.Common;
using Project.Scripts.Items.Equipments;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.Animations;

public class AttackAnimationEventProxy : MonoBehaviour {
    [NotNull] 
    [field: SerializeField, Required] 
    private ComboAttack? AttackComposer { get; set; }
    
    private Dictionary<EquipmentSlot, EquipmentSocket> EquipmentSockets { get; set; } = [];
    private DamageDealer? CurrentWeapon { get; set; }

    private void Awake() {
        foreach (EquipmentSocket socket in this.GetComponentsInChildren<EquipmentSocket>()) {
            this.EquipmentSockets.Add(socket.Slot, socket);       
        }
    }

    public void OnAttackStart(EquipmentSlot where) {
        if (!this.EquipmentSockets.TryGetValue(where, out EquipmentSocket socket) || !socket.Equipment) {
            return;
        }

        if (!socket.Equipment.TryGetComponent(out DamageDealer weapon)) {
            return;
        }

        this.CurrentWeapon = weapon;
        this.transform.root.GetComponentInChildren<PlayerAudioComponent>().Play(PlayerAudioComponent.Sound.Attack);
        weapon.enabled = true;
    }

    public void OnAttackPerformed(EquipmentSlot where) {
        if (this.CurrentWeapon) {
            this.CurrentWeapon.TryPerformHit();
            this.CurrentWeapon = null;
        }

        this.AttackComposer.IsAttacking = false;
    }
}
