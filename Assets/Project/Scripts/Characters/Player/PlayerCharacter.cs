using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Characters.CharacterControl;
using Project.Scripts.Combat;
using Project.Scripts.Common;
using Project.Scripts.InventorySystem;
using Project.Scripts.Items;
using Project.Scripts.Items.Equipments;
using Project.Scripts.Util.ChainOfResponsibilities;
using Project.Scripts.Util.Linq;
using UnityEngine;

namespace Project.Scripts.Characters.Player;

[RequireComponent(typeof(AttributeSet), typeof(Inventory))]
public class PlayerCharacter : MonoBehaviour, IConsumer {
    [field: SerializeField]
    private NewPlayerPreset? NewPlayerPreset { get; set; }
    
    [NotNull]
    private AttributeSet? AttributeSet { get; set; }
    
    [NotNull]
    private Inventory? Inventory { get; set; }
    
    [NotNull]
    private ComboAttack? ComboAttack { get; set; }
    
    [NotNull]
    private EquipmentSystem? EquipmentSystem { get; set; }

    private IProcessor<ItemProcessingArgs> ItemConsumer { get; init; } =
        Processor<ItemProcessingArgs>.Builder
                                     .StartWith<EquipmentProcessor>()
                                     .IfNotDoneThen<GeneralItemProcessor>()
                                     .Build();

    private void Awake() {
        this.AttributeSet = this.GetComponent<AttributeSet>();
        this.Inventory = this.GetComponent<Inventory>();
        this.ComboAttack = this.GetComponent<ComboAttack>();
        this.EquipmentSystem = this.GetComponent<EquipmentSystem>();
    }

    private void Start() {
        this.Initialise();
    }

    public void Initialise() {
        GameEvents.OnNotification += this.OnNotification;
        
        PlayerInputInterpreter input = this.GetComponent<PlayerInputInterpreter>();
        input.OnCommitRightHandAttack += this.ComboAttack.Commit;
        input.OnOpenInventory += this.Inventory.Open;
        
        this.Inventory.OnUseItem += this.Consume;
        this.EquipmentSystem.OnEquip += equipment => this.AttributeSet.Accept(equipment);
        this.EquipmentSystem.OnUnequip += equipment => equipment.UnequipFrom(this.AttributeSet);
        
        this.InitialiseAttributes();
    }

    private void OnNotification(GameNotification msg) {
        switch (msg) {
            case GameNotification.ComboJustStarted:
                this.ComboAttack.StartCombo();
                break;
            case GameNotification.ComboHasEnded:
                this.ComboAttack.EndCombo();
                break;
        }
    }

    private void InitialiseAttributes() {
        if (!this.NewPlayerPreset) {
            return;
        }

        foreach (CharacterAttributeData data in this.NewPlayerPreset.InitialStats) {
            if (data.IsCappedByValue) {
                this.AttributeSet.Init(data.AttributeType, data.Value, maxValue: data.MaxValue);
            } else if (data.IsCappedByAttribute) {
                this.AttributeSet.Init(data.Cap, data.Value);
                this.AttributeSet.Init(data.AttributeType, data.Value, cap: data.Cap);
            } else {
                this.AttributeSet.Init(data.AttributeType, data.Value);
            }
        }

        foreach (KeyValuePair<Item, int> entry in this.NewPlayerPreset.StartingInventory) {
            this.Inventory.Add(entry);
        }
    }

    public void Consume(Inventory from, Item item) {
        this.ItemConsumer.Process(new ItemProcessingArgs(item, this.AttributeSet.Accept, this.EquipmentSystem.Accept));
    }
}
