using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cysharp.Threading.Tasks;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.SaveGames;
using Project.Scripts.Util.Singleton;
using SaintsField;
using SaintsField.Playa;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.Common;

internal class GameInitiator : Singleton<GameInitiator> {
    [NotNull]
    [field: LayoutStart("Singletons", ELayout.Foldout)]
    [field: SerializeField]
    private SaveGameManager? SaveGameManager { get; set; }
    
    [NotNull]
    [field: SerializeField]
    private LevelManager? LevelManager { get; set; }
    
    [NotNull]
    [field: LayoutStart("Environment", ELayout.Foldout)]
    [field: SerializeField]
    private Camera? MainCamera { get; set; }
    
    [NotNull]
    [field: SerializeField]
    private CinemachineCamera? CinemachineCamera { get; set; }
    
    [NotNull]
    [field: LayoutEnd]
    [field: SerializeField, Header("Player")]
    private GameObject? Player { get; set; }
    
    /*[field: SerializeField]
    private NewPlayerPreset? NewPlayerPreset { get; set; }*/
    
    private async void Start() {
        /*try {
            UniTask instantiation = this.InstantiateObjects();
            await instantiation;
            this.InitialiseObjects();
            await this.LoadResources();
            await this.PrepareGameData();
            await this.BeginGame();
        } catch (Exception e) {
            Debug.Log("Failed to initialise game.");
            Debug.LogException(e);
        }*/
    }

    private async UniTask InstantiateObjects() {
        AsyncInstantiateOperation<SaveGameManager> task1 = Object.InstantiateAsync(this.SaveGameManager);
        AsyncInstantiateOperation<LevelManager> task2 = Object.InstantiateAsync(this.LevelManager);
        AsyncInstantiateOperation<Camera> task3 = Object.InstantiateAsync(this.MainCamera);
        AsyncInstantiateOperation<CinemachineCamera> task4 = Object.InstantiateAsync(this.CinemachineCamera);
        
        this.SaveGameManager = (await task1).Single();
        this.LevelManager = (await task2).Single();
        this.MainCamera = (await task3).Single();
        this.CinemachineCamera = (await task4).Single();
    }

    private void InitialiseObjects() { }

    private async UniTask LoadResources() {
        this.Player = (await Object.InstantiateAsync(this.Player)).Single();
    }

    private async UniTask PrepareGameData() {
        /*if (this.NewPlayerPreset) {
            AttributeSet attributes = this.Player.GetComponent<AttributeSet>();
            foreach (CharacterAttributeData stat in this.NewPlayerPreset.InitialStats) {
                attributes.Init(stat.AttributeType, stat.Value);
            }
        }*/
    }

    private async UniTask BeginGame() {
        
    }
}