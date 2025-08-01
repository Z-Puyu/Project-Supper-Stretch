using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control.MVP.Components;

[DisallowMultipleComponent]
public class MainMenu : MonoBehaviour {
    [NotNull] [field: SerializeField] private Button? StartButton { get; set; }
    [NotNull] [field: SerializeField] private Button? QuitButton { get; set; }

    private void Start() {
        this.StartButton.onClick.AddListener(() => SceneManager.LoadScene(1));
        this.QuitButton.onClick.AddListener(Application.Quit);
    }
}
