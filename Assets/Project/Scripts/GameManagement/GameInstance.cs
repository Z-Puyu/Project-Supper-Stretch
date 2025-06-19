using System.Diagnostics.CodeAnalysis;
using DunGen.Project.External.DunGen.Code;
using Project.Scripts.Characters.CharacterControl.Combat;
using Project.Scripts.Characters.Enemies;
using Project.Scripts.Characters.Player;
using Project.Scripts.Map;
using Project.Scripts.Util.Linq;
using SaintsField.Playa;
using Project.Scripts.Util.Singleton;
using Unity.Cinemachine;
using UnityEngine;

namespace Project.Scripts.GameManagement;

public class GameInstance : Singleton<GameInstance> {
    [NotNull]
    [field: SerializeField, LayoutStart("Level Map", ELayout.Foldout)]
    private GameMap? MapGenerator { get; set; }
    
    [NotNull]
    [field: LayoutStart("Environment", ELayout.Foldout)]
    [field: SerializeField]
    private Camera? MainCamera { get; set; }
    
    [NotNull]
    [field: SerializeField]
    private CinemachineCamera? CinemachineCamera { get; set; }
    
    [NotNull]
    [field: LayoutEnd, SerializeField, Header("Player")]
    private GameObject? Player { get; set; }
    
    [NotNull]
    [field: SerializeField, LayoutEnd, Header("UI")]
    private Canvas? UI { get; set; }
    
    [NotNull] public Transform? Eyes { get; private set; }
    private GameMap? StartingMap { get; set; }
    private CinemachineCamera? VirtualCamera { get; set; }
    [NotNull] public PlayerCharacter? PlayerInstance { get; private set; }
    private PlayerInputInterpreter? Input { get; set; }
    
    private void Start() {
        this.InstantiateObjects();
        this.InitialiseObjects();
        this.InitialiseLevel();
    }

    private void InitialiseUI() {
        Object.Instantiate(this.UI);
    }

    private void InstantiateObjects() {
        this.StartingMap = Object.Instantiate(this.MapGenerator);
        Object.Instantiate(this.MainCamera);
        this.Eyes = Camera.main!.transform;
        this.VirtualCamera = Object.Instantiate(this.CinemachineCamera);
        this.PlayerInstance = Object.Instantiate(this.Player).GetComponent<PlayerCharacter>();
        this.Input = this.PlayerInstance.GetComponent<PlayerInputInterpreter>();
    }

    private void InitialiseObjects() {
        this.VirtualCamera!.Target.TrackingTarget = this.PlayerInstance!.transform;
        this.PlayerInstance.Initialise();
    }
    
    private void InitialiseLevel() {
        this.StartingMap!.Generate(this.PrepareGame);
    }

    private void PrepareGame(DungeonGenerator dungeon) {
        Transform player = this.PlayerInstance.transform;
        player.position = dungeon.Root.transform.position;
        player.rotation = dungeon.Root.transform.rotation;
        dungeon.Root.GetComponentsInChildren<EnemyCharacter>().ForEach(enemy => enemy.Initialise());
        this.InitialiseUI();
        this.BeginGame();
    }

    private void BeginGame() {
        this.Input!.enabled = true;
    }
}