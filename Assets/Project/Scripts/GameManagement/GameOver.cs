using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.GameManagement;

[DisallowMultipleComponent]
public class GameOver : MonoBehaviour {
    [field: SerializeField] private Button? RestartButton { get; set; }

    private void Start() {
        if (this.RestartButton) {
            this.RestartButton.onClick.AddListener(GameOver.Restart);     
        }
        
        this.gameObject.SetActive(false);
    }

    private static void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
