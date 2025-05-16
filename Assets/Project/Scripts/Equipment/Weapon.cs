using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.Events;
using UnityEngine;

namespace Project.Scripts.Equipment;

[RequireComponent(typeof(Collider))]
public class Weapon : MonoBehaviour {
    [NotNull]
    private Collider? Collider { get; set; }
    
    [field: SerializeField]
    private EventChannel<GameplayEffectReceiver>? OnAttack { get; set; }

    private void Awake() {
        this.Collider = this.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) {
        GameplayEffectReceiver? target = other.GetComponent<GameplayEffectReceiver>();
        if (target == null) {
            return;
        }
        
        this.Collider.enabled = false;
        this.OnAttack?.Broadcast(this, target);
    }
}
