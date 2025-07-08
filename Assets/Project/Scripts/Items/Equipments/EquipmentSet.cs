using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Common;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Items.Equipments;

[DisallowMultipleComponent]
public class EquipmentSet : MonoBehaviour {
    [NotNull] 
    [field: SerializeField, Required] 
    public GameObject? Root { get; private set; }
    
    [field: SerializeField] private AttributeSet? AttributeSetComponent { get; set; }
    
    private Dictionary<Item, EquipmentSocket> EquipmentLookup { get; init; } = [];
    private SortedList<EquipmentSlot, EquipmentSocket> Sockets { get; init; } = [];
    
    [field: SerializeField, LayoutStart("Animator Config", ELayout.Foldout)] 
    private Animator? Animator { get; set; }
    
    [field: SerializeField, AnimatorParam(nameof(this.Animator), AnimatorControllerParameterType.Bool)]
    private int DrawWeaponCondition { get; set; }
    
    public event UnityAction<GameplayEffect, IEnumerable<Modifier>>? OnEquipmentChanged; 
    public event UnityAction? OnEquip;
    public event UnityAction? OnUnequip;

    private void Awake() {
        foreach (EquipmentSocket socket in this.Root.GetComponentsInChildren<EquipmentSocket>()) {
            if (this.Sockets.TryAdd(socket.Slot, socket)) {
                continue;
            }

            Debug.LogError($"Duplicate socket for {socket.Slot} found in {this.Root.name}!", this);
            return;
        }

        if (this.AttributeSetComponent) {
            this.OnEquipmentChanged += (effect, modifiers) => {
                GameplayEffectExecutionArgs args = GameplayEffectExecutionArgs.Builder.From(this.AttributeSetComponent)
                                                                              .WithCustomModifiers(modifiers).Build();
                this.AttributeSetComponent.AddEffect(effect, args);
            };
        }
    }

    private void OnDestroy() {
        this.OnEquipmentChanged = null;
        this.OnEquip = null;
        this.OnUnequip = null;
    }

    public bool HasAny<T>(Predicate<Item>? predicate = null) where T : Component {
        return this.Sockets.Values.Any(socket => socket.Holds<T>(predicate));
    }

    public bool HasAny<T>(out T? component, Predicate<Item>? predicate = null) where T : Component {
        foreach (EquipmentSocket socket in this.Sockets.Values) {
            if (!socket.Holds(out T? equipped, predicate)) {
                continue;
            }

            component = equipped!;
            return true;
        }

        component = null;
        return false;
    }
    
    public bool Equip(in Item equipment, in EquipmentProperty property, out EquipmentSocket socket) {
        EquipmentSlot slot = property.Slot;
        EquipmentSocket[] sockets = this.Sockets.Values.Where(socket => slot.HasFlag(socket.Slot)).ToArray();
        if (!sockets.Any()) {
            Logging.Error($"{this.gameObject.name} has no socket with slot {slot}", this);
            socket = this.Sockets[0];
            return false;       
        }
        
        EquipmentSocket? available = sockets.FirstOrDefault(socket => socket.IsAvailable);
        if (!available) {
            socket = sockets[0];
            return false;       
        }
        
        available.Attach(equipment, property.Model);
        this.EquipmentLookup[equipment] = available;
        socket = available;
        if (equipment.Type.HasFlag(ItemFlag.Weapon) && this.Animator) {
            this.Animator.SetBool(this.DrawWeaponCondition, true);
        }
        
        this.OnEquipmentChanged?.Invoke(property.GameplayEffectOnEquip, property.Modifiers);
        this.OnEquip?.Invoke();
        equipment.IsEquipped = true;
        return true;
    }

    public bool Unequip(in Item equipment, in EquipmentProperty property) {
        if (!this.EquipmentLookup.TryGetValue(equipment, out EquipmentSocket socket)) {
            return false;
        }
        
        socket.Detach();
        this.EquipmentLookup.Remove(equipment);
        if (equipment.Type.HasFlag(ItemFlag.Weapon) && this.Animator) {
            this.Animator.SetBool(this.DrawWeaponCondition, false);
        }

        this.OnEquipmentChanged?.Invoke(property.GameplayEffectOnUnequip, property.Modifiers.Select(m => -m));
        this.OnUnequip?.Invoke();
        equipment.IsEquipped = false;
        return true;
    }
}
