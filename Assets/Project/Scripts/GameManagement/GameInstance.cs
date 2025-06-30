using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Project.Scripts.Characters;
using Project.Scripts.Characters.Player;
using Project.Scripts.Common;
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
    
    [field: SerializeField] private GameObject? PlayerHUD { get; set; }
    [field: SerializeField] private LoadingScreen? LoadingScreen { get; set; }
    
    [NotNull] public Transform? Eyes { get; private set; }
    [NotNull] private GameMap? StartingMap { get; set; }
    private CinemachineCamera? VirtualCamera { get; set; }
    [NotNull] public PlayerCharacter? PlayerInstance { get; private set; }
    [NotNull] private LoadingScreen? LoadingScreenInstance { get; set; }
    
    private void Start() {
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

    private void InitialiseUI() {
        Object.Instantiate(this.UI);
        Object.Instantiate(this.PlayerHUD);
    }

    private void InstantiateObjects() {
        this.LoadingScreenInstance.FlashHintText("Creating Objects...");
        Object.Instantiate(this.MainCamera);
        this.Eyes = Camera.main!.transform;
        this.VirtualCamera = Object.Instantiate(this.CinemachineCamera);
        this.StartingMap = Object.Instantiate(this.MapGenerator);
        this.PlayerInstance = Object.Instantiate(this.Player).GetComponent<PlayerCharacter>();
    }

    private void InitialiseObjects() {
        this.LoadingScreenInstance.FlashHintText("Initialising Objects...");
    }
    
    private void InitialiseLevel() {
        this.LoadingScreenInstance.FlashHintText("Generating Maps...");
        Object.FindAnyObjectByType<NavMeshSurface>().BuildNavMesh();
        this.StartingMap.Begin(_ => this.BeginGame());
    }

    private void BeginGame() {
        this.LoadingScreenInstance.FlashHintText("Enabling Scripts...");
        Object.FindObjectsByType<GameCharacter>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
              .ForEach(character => character.transform.SetParent(null));
        Object.FindObjectsByType<NavMeshAgent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
              .ForEach(agent => agent.enabled = true);
        Object.FindObjectsByType<BehaviorGraphAgent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
              .ForEach(agent => agent.enabled = true);
        PlayerCharacter.OnDungeonLevelCleared += this.StartingMap.Generate;
        this.VirtualCamera!.Target.TrackingTarget =
                this.PlayerInstance.GetComponentInChildren<CameraTarget>().transform;
        LeanTween.alphaCanvas(this.LoadingScreenInstance.GetComponent<CanvasGroup>(), 0, 2f)
                 .setOnComplete(() => {
                     this.LoadingScreenInstance.gameObject.SetActive(false);
                     this.PlayerInstance.EnableInput();
                 });
    }
}