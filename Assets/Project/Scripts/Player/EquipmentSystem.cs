using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Project.Scripts.Player;

public class EquipmentSystem : MonoBehaviour {
    [NotNull]
    [field: SerializeField]
    private EquipmentSocket? WeaponSocket { get; set; }
    
    [field: SerializeField]
    private GameObject? TestWeapon { set; get; }

    private void Start() {
        if (this.TestWeapon != null) {
            this.Equip(this.TestWeapon);
        }
    }

    public void Equip(GameObject equipment) {
        this.WeaponSocket.Attach(equipment);
    }
}