using CommonFrameworks.Utilities;
using SaintsField;
using SaveAndLoadSystem.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement {
    public sealed class GameInstance : Singleton<GameInstance> {
        [field: SerializeField, Scene] private int GameWorldScene { get; set; }
        
        public void StartNewGame() {
            Singleton<SaveGameSystem>.Instance.NewGame();
            SceneManager.LoadSceneAsync(this.GameWorldScene);
        }
    }
}