using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Characters.CharacterControl;
using Project.Scripts.Combat;
using Project.Scripts.Common;
using Project.Scripts.InteractionSystem;
using Project.Scripts.Items;
using UnityEngine;

namespace Project.Scripts.Characters.Player;

[RequireComponent(typeof(Interactor))]
public class PlayerCharacter : GameCharacter<NewPlayerPreset> {
    [NotNull]
    private ComboAttack? ComboAttack { get; set; }
    
    [NotNull]
    private Interactor? Interactor { get; set; }

    protected override void Awake() {
        base.Awake();
        this.ComboAttack = this.GetComponent<ComboAttack>();
        this.Interactor = this.GetComponent<Interactor>();
    }
    
    protected void Start() {
        this.Initialise();
    }

    public override void Initialise() {
        base.Initialise();
        this.InitialiseInventory();
        this.InitialiseInput();
    }

    private void InitialiseInput() {
        GameEvents.OnNotification += this.OnNotification;
        PlayerInputInterpreter input = this.GetComponent<PlayerInputInterpreter>();
        
        input.OnCommitRightHandAttack += this.ComboAttack.Commit;
        input.OnOpenInventory += this.Inventory.Open;
        input.OnInteract += this.Interactor.Interact;
    }

    private void InitialiseInventory() {
        foreach (KeyValuePair<ItemData, int> entry in this.CharacterData!.StartingInventory) {
            this.Inventory.Add(Item.From(entry.Key), entry.Value);
        }
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
}
