using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DunGen;
using Project.Scripts.Characters.Enemies;
using Project.Scripts.Common;
using Project.Scripts.Common.Input;
using Project.Scripts.Interaction;
using Project.Scripts.Items;
using Project.Scripts.Items.Equipments;
using Project.Scripts.Util.Linq;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Project.Scripts.Characters.Player;

[RequireComponent(typeof(Interactor), typeof(EquipmentSystem), typeof(ExperienceSystem))]
public class PlayerCharacter : GameCharacter<NewPlayerPreset> {
    public static event UnityAction OnDungeonLevelCleared = delegate { }; 
    
    [NotNull] public InputActions? InputActions { get; private set; }
    [NotNull] private Interactor? Interactor { get; set; }
    [NotNull] private EquipmentSystem? EquipmentSystem { get; set; }
    [NotNull] private ExperienceSystem? ExperienceSystem { get; set; }
    [NotNull] [field: SerializeField] private PlayerMovement? Movement { get; set; }

    #region Debug

    [field: SerializeField] private bool IsInvincible { get; set; }

    #endregion

    protected override void Awake() {
        base.Awake();
        this.InputActions = new InputActions();
        this.Interactor = this.GetComponent<Interactor>();
        this.EquipmentSystem = this.GetComponent<EquipmentSystem>();
        this.ExperienceSystem = this.GetComponent<ExperienceSystem>();
    }

    protected override void Start() {
        base.Start();
        foreach (KeyValuePair<ItemData, int> entry in this.CharacterData.StartingInventory) {
            this.Inventory.Add(Item.From(entry.Key), entry.Value);
        }
        
        this.GetComponentsInChildren<IPlayerControllable>().ForEach(control => control.BindInput(this.InputActions));
        Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
              .OfType<IUserInterface>()
              .ForEach(control => control.BindInput(this.InputActions));
        GameEvents.OnPause += enterUI;
        GameEvents.OnPlay += exitUI;
        this.InputActions.Player.Enable();
        GameCharacter<Enemy>.OnDeath += this.OnFindDeadEnemy;
        this.GetComponent<DungenCharacter>().OnTileChanged += PlayerCharacter.OnEnterDungeonRoom;
        return;

        void enterUI() {
            this.InputActions.Player.Disable();
            this.InputActions.UI.Enable();
        }

        void exitUI() {
            this.InputActions.UI.Disable();
            this.InputActions.Player.Enable();
        }
    }

    private static void OnEnterDungeonRoom(DungenCharacter character, Tile from, Tile to) {
        if (to.Dungeon && to.Dungeon.MainPathTiles[^1] == to) {
            PlayerCharacter.OnDungeonLevelCleared.Invoke();
        }
    }

    private void OnFindDeadEnemy(Enemy corpse, GameObject? killer) {
        if (killer == this.gameObject) {
            this.ExperienceSystem.AddExperience(corpse.Experience);
        }
    }

    protected override void DyingFrom(GameObject? source) {
        if (this.IsInvincible) {
            Logging.Info($"{this.gameObject.name} is invincible (DEBUG MODE)", this);
            return;
        }
        
        base.DyingFrom(source);
    }
}
