using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DunGen;
using Project.Scripts.Characters.Enemies;
using Project.Scripts.Common;
using Project.Scripts.Common.Input;
using Project.Scripts.Items;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.Util.Components;
using Project.Scripts.Util.Linq;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Project.Scripts.Characters.Player;

public class PlayerCharacter : GameCharacter<NewPlayerPreset> {
    public static event UnityAction? OnDungeonLevelCleared; 
    
    [NotNull] public InputActions? InputActions { get; private set; }
    [NotNull] [field: SerializeField] private PlayerMovement? Movement { get; set; }
    [NotNull] [field: SerializeField] private ExperienceSystem? ExperienceSystem { get; set; }

    #region Debug

    [field: SerializeField] private bool IsInvincible { get; set; }

    #endregion

    protected override void Awake() {
        base.Awake();
        this.InputActions = new InputActions();
    }

    public void EnableInput() {
        this.InputActions.Player.OpenPauseMenu.performed += _ => GameEvents.UI.OnOpenPauseMenu?.Invoke();
        this.GetComponentsInChildren<IPlayerControllable>().ForEach(control => control.BindInput(this.InputActions));
        Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
              .OfType<IUserInterface>()
              .ForEach(control => control.BindInput(this.InputActions));
        Logging.Info("Input bindings enabled", this);
    }

    protected override void Start() {
        base.Start();
        if (this.HasChildComponent(out Inventory inventory)) {
            foreach (KeyValuePair<ItemData, int> entry in this.CharacterData.StartingInventory) {
                inventory.Add(Item.From(entry.Key), entry.Value);
            }
        }
        
        this.OnPlay();
        this.GetComponent<DungenCharacter>().OnTileChanged += PlayerCharacter.OnEnterDungeonRoom;
        GameCharacter<Enemy>.OnDeath += this.CheckDeadEnemy;
        Logging.Info("Player started", this);
    }

    private void CheckDeadEnemy(Enemy enemy, GameObject? killer) {
        if (killer && killer.transform.IsChildOf(this.gameObject.transform)) {
            this.ExperienceSystem.AddExperience(enemy.Experience);
        }
    }

    public void InitialiseComponents() {
        this.GetComponentInChildren<PhysicalConditions>().Initialise();
        this.GetComponentInChildren<PhysicalConditions>().CheckInitialConditions();
        Logging.Info("Components initialised", this);
    }
    
    protected override void OnPause() {
        base.OnPause();
        this.InputActions.Player.Disable();
        this.InputActions.UI.Enable();
        Cursor.visible = true;
    }
    
    protected override void OnPlay() {
        base.OnPlay();
        this.InputActions.Player.Enable();
        this.InputActions.UI.Disable();
        Cursor.visible = false;
    }

    private static void OnEnterDungeonRoom(DungenCharacter character, Tile from, Tile to) {
        if (to.Dungeon && to.Dungeon.MainPathTiles[^1] == to) {
            PlayerCharacter.OnDungeonLevelCleared?.Invoke();
        }
    }

    protected override void DyingFrom(GameObject? source) {
        if (this.IsInvincible) {
            Logging.Info($"{this.gameObject.name} is invincible (DEBUG MODE)", this);
            return;
        }

        this.GetComponentsInChildren<IPlayerControllable>(includeInactive: true)
            .ForEach(component => component.UnbindInput(this.InputActions));
        this.InputActions.Player.Disable();
        base.DyingFrom(source);
    }

    public override void Kill() {
        this.InputActions.Dispose();
        base.Kill();
    }

    private void OnDisable() {
        this.InputActions.Disable();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        this.GetComponentsInChildren<IPlayerControllable>(includeInactive: true)
            .ForEach(component => component.UnbindInput(this.InputActions));
        GameCharacter<Enemy>.OnDeath -= this.CheckDeadEnemy;
    }
}
