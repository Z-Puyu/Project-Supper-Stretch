using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Characters.CharacterControl;
using Project.Scripts.Characters.CharacterControl.Combat;
using Project.Scripts.Characters.Enemies;
using Project.Scripts.Interaction;
using Project.Scripts.Items;
using Project.Scripts.Items.Equipments;
using UnityEngine;

namespace Project.Scripts.Characters.Player;

[RequireComponent(typeof(Interactor), typeof(EquipmentSystem), typeof(ExperienceSystem))]
[RequireComponent(typeof(PlayerInputInterpreter))]
public class PlayerCharacter : GameCharacter<NewPlayerPreset> {
    [NotNull] private Interactor? Interactor { get; set; }
    [NotNull] private EquipmentSystem? EquipmentSystem { get; set; }
    [NotNull] private ExperienceSystem? ExperienceSystem { get; set; }
    [NotNull] private PlayerInputInterpreter? InputInterpreter { get; set; }
    [NotNull] [field: SerializeField] private PlayerMovement? Movement { get; set; }

    #region Debug

    [field: SerializeField] private bool IsInvincible { get; set; }

    #endregion

    protected override void Awake() {
        base.Awake();
        this.Interactor = this.GetComponent<Interactor>();
        this.EquipmentSystem = this.GetComponent<EquipmentSystem>();
        this.ExperienceSystem = this.GetComponent<ExperienceSystem>();
    }

    public override void Initialise() {
        base.Initialise();
        this.InitialiseInventory();
        this.InitialiseInput();
        this.EquipmentSystem.OnEquip += this.ComboAttack.RegisterWeapon;
        this.EquipmentSystem.OnUnequip += this.ComboAttack.ForgetWeapon;
        GameCharacter<Enemy>.OnDeath += this.OnFindDeadEnemy;
    }

    private void OnFindDeadEnemy(Enemy corpse, GameObject? killer) {
        if (killer == this.gameObject) {
            this.ExperienceSystem.AddExperience(corpse.Experience);
        }
    }

    private void InitialiseInput() {
        this.InputInterpreter = this.GetComponent<PlayerInputInterpreter>();
        this.InputInterpreter.OnMove += this.Movement.MoveTowards;
        this.InputInterpreter.OnWalk += () => this.Movement.SwitchMode(PlayerMovement.Mode.Walk);
        this.InputInterpreter.OnRun += () => this.Movement.SwitchMode(PlayerMovement.Mode.Run);
        this.InputInterpreter.OnSprint += () => this.Movement.SwitchMode(PlayerMovement.Mode.Sprint);
        this.InputInterpreter.OnStop += this.Movement.StopImmediately;
        this.InputInterpreter.OnToggleWalkingLock += () => this.Movement.Locked = !this.Movement.Locked;
        this.InputInterpreter.OnCommitRightHandAttack += this.ComboAttack.CommitNextStage;
        this.InputInterpreter.OnOpenInventory += this.Inventory.Open;
        this.InputInterpreter.OnInteract += this.Interactor.Interact;
    }

    private void InitialiseInventory() {
        foreach (KeyValuePair<ItemData, int> entry in this.CharacterData!.StartingInventory) {
            this.Inventory.Add(Item.From(entry.Key), entry.Value);
        }
    }

    protected override void DyingFrom(GameObject? source) {
        if (this.IsInvincible) {
            Debug.Log($"{this.gameObject.name} is invincible (DEBUG MODE)");
            return;
        }
        
        this.InputInterpreter.enabled = false;
        this.Movement.enabled = false;
        this.Interactor.enabled = false;
        this.EquipmentSystem.enabled = false;
        this.ExperienceSystem.enabled = false;
        base.DyingFrom(source);
    }
}
