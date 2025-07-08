using System;
using System.Diagnostics.CodeAnalysis;
using DunGen;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.Characters;
using Project.Scripts.Characters.Enemies;
using Project.Scripts.Characters.Player;
using Project.Scripts.Common;
using Project.Scripts.Items.CraftingSystem;
using Project.Scripts.Items.Definitions;
using Project.Scripts.Map;
using Project.Scripts.Util.Linq;
using SaintsField.Playa;
using Project.Scripts.Util.Singleton;
using Unity.AI.Navigation;
using Unity.Behavior;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using CameraTarget = Project.Scripts.Characters.Player.CameraTarget;
using Object = UnityEngine.Object;

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
    private GameObject? UI { get; set; }
    
    [field: SerializeField] private GameOver? GameWinScreen { get; set; }
    [field: SerializeField] private GameOver? GameOverScreen { get; set; }
    [field: SerializeField] private GameObject? PlayerHUD { get; set; }
    [field: SerializeField] private LoadingScreen? LoadingScreen { get; set; }
    
    [NotNull] public Transform? Eyes { get; private set; }
    [NotNull] private GameMap? StartingMap { get; set; }
    private CinemachineCamera? VirtualCamera { get; set; }
    [NotNull] public PlayerCharacter? PlayerInstance { get; private set; }
    [NotNull] public Transform? PlayerTransform { get; private set; }
    [NotNull] private LoadingScreen? LoadingScreenInstance { get; set; }
    [NotNull] private GameOver? GameOverScreenInstance { get; set; }
    [NotNull] private GameOver? GameWinScreenInstance { get; set; }
    private NavMeshSurface? NavMesh { get; set; }

    private void Start() {
        GameEvents.OnPause = delegate { };
        GameEvents.OnPlay = delegate { };
        GameEvents.UI.OnOpenPauseMenu = delegate { };
        Resources.LoadAll<ItemDefinition>("").ForEach(asset => asset.name = asset.name);
        Resources.LoadAll<AttributeDefinition>("").ForEach(asset => asset.name = asset.name);
        Resources.LoadAll<SchemeDefinition>("").ForEach(asset => asset.name = asset.name);
        this.LoadGame();
    }

    public void LoadGame() {
        this.ShowLoadingScreen();
        this.InstantiateObjects();
        this.InitialiseObjects();
        this.InitialiseLevel();
        this.InitialiseUI();
    }

    private void ShowLoadingScreen() {
        this.LoadingScreenInstance = Object.Instantiate(this.LoadingScreen);
        this.LoadingScreenInstance.FlashHintText("Loading...");
        this.LoadingScreenInstance.gameObject.SetActive(true);
    }

    private void CheckGameVictory(Enemy deadEnemy, GameObject? _) {
        if (!deadEnemy.IsBoss) {
            return;
        }

        this.GameWinScreenInstance.gameObject.SetActive(true);
        Cursor.visible = true;
        GameEvents.OnPause?.Invoke();
    }

    private void InitialiseUI() {
        Object.Instantiate(this.UI);
        Object.Instantiate(this.PlayerHUD);
        this.GameOverScreenInstance = Object.Instantiate(this.GameOverScreen);
        this.GameWinScreenInstance = Object.Instantiate(this.GameWinScreen);
        this.PlayerInstance.OnKilled += () => {
            Cursor.visible = true;
            this.GameOverScreenInstance.gameObject.SetActive(true);
        };

        GameCharacter<Enemy>.OnDeath += this.CheckGameVictory;
    }

    private void InstantiateObjects() {
        this.LoadingScreenInstance.FlashHintText("Creating Objects...");
        Object.Instantiate(this.MainCamera);
        this.Eyes = Camera.main!.transform;
        this.VirtualCamera = Object.Instantiate(this.CinemachineCamera);
        this.PlayerInstance = Object.Instantiate(this.Player).GetComponent<PlayerCharacter>();
        this.StartingMap = Object.Instantiate(this.MapGenerator);
        this.PlayerTransform = this.PlayerInstance.transform;
        this.PlayerTransform.SetParent(GameObject.FindGameObjectWithTag("PlayerStart").transform);
        this.PlayerTransform.localPosition = Vector3.zero;
        this.PlayerTransform.localRotation = Quaternion.identity;
        Logging.Info("Instantiating Objects... Done.", this);
    }

    private void InitialiseObjects() {
        this.LoadingScreenInstance.FlashHintText("Initialising Objects...");
    }
    
    private void InitialiseLevel() {
        this.LoadingScreenInstance.FlashHintText("Generating Maps...");
        this.NavMesh = Object.FindAnyObjectByType<NavMeshSurface>();
        this.StartingMap.GetComponentInChildren<Tile>().RecalculateBounds();
        this.StartingMap.Begin(_ => this.BeginGame());
        Logging.Info("Initialising Map... Done.", this);
    }

    private void BeginGame() {
        this.LoadingScreenInstance.FlashHintText("Enabling Scripts...");
        this.NavMesh!.BuildNavMesh();
        this.StartingMap.GetComponentsInChildren<GoalPoint>(includeInactive: true)
            .ForEach(point => point.gameObject.SetActive(true));
        Object.FindObjectsByType<GameCharacter>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
              .ForEach(character => character.transform.SetParent(null));
        Object.FindObjectsByType<NavMeshAgent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
              .ForEach(agent => agent.enabled = true);
        Object.FindObjectsByType<BehaviorGraphAgent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
              .ForEach(agent => agent.enabled = true);
        // PlayerCharacter.OnDungeonLevelCleared += this.StartingMap.Generate;
        this.VirtualCamera!.Target.TrackingTarget =
                this.PlayerInstance.GetComponentInChildren<CameraTarget>().transform;
        this.PlayerInstance.InitialiseComponents();
        LeanTween.alphaCanvas(this.LoadingScreenInstance.GetComponent<CanvasGroup>(), 0, 2f)
                 .setOnComplete(() => {
                     this.LoadingScreenInstance.gameObject.SetActive(false);
                     this.PlayerInstance.EnableInput();
                 });
        Logging.Info("Enabling Scripts... Done.", this);
    }

    private void OnDestroy() {
        // PlayerCharacter.OnDungeonLevelCleared -= this.StartingMap.Generate;
        GameCharacter<Enemy>.OnDeath -= this.CheckGameVictory;
    }
}