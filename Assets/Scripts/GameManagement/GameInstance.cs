using CommonFrameworks.Utilities;
using SaintsField;
using SaveAndLoad;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement {
    public sealed class GameInstance : Singleton<GameInstance> {
        [field: SerializeField, Scene] private int GameWorldScene { get; set; }
        
        public void StartNewGame() {
            if (Singleton<SaveGameSystem>.Instance.NewGame(0, "Current Game")) {
                SceneManager.LoadSceneAsync(this.GameWorldScene);
            }
        }

        public void ContinueGame() {
            if (Singleton<SaveGameSystem>.Instance.LoadLatestSave()) {
                SceneManager.LoadSceneAsync(this.GameWorldScene);
            }
        }

        private void OnApplicationQuit() {
            if (SceneManager.GetActiveScene().buildIndex == this.GameWorldScene) {
                Singleton<SaveGameSystem>.Instance.Save(0);
            }
        }
    }
}