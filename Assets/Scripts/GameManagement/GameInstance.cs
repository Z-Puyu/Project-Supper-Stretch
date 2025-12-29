using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement {
    public sealed class GameInstance : Singleton<GameInstance> {
        [field: SerializeField, Scene] private int GameWorldScene { get; set; }
        
        public void StartNewGame() {
            SceneManager.LoadSceneAsync(this.GameWorldScene);
        }
    }
}