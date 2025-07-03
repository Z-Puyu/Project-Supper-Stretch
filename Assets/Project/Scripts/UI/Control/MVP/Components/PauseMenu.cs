using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.GameManagement;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.UI.Control.MVP.Presenters;
using Project.Scripts.Util.Components;
using Project.Scripts.Util.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control.MVP.Components;

[DisallowMultipleComponent]
public class PauseMenu : MonoBehaviour, IPresenter {
    [NotNull] [field: SerializeField] private UIBook? Book { get; set; }
    [NotNull] [field: SerializeField] private Button? ToPlayerAttributeButton { get; set; }
    [NotNull] [field: SerializeField] private Button? ToInventoryButton { get; set; }
    [NotNull] [field: SerializeField] private Button? ResumeButton { get; set; }
    [NotNull] [field: SerializeField] private Button? RestartButton { get; set; }
    [NotNull] [field: SerializeField] private Button? QuitButton { get; set; }

    private void Start() {
        if (Singleton<GameInstance>.Instance.PlayerInstance.HasChildComponent(out AttributeSet attribute)) {
            this.ToPlayerAttributeButton.onClick.AddListener(() => {
                this.Book.PreviousPage();
                this.Book.Open<PlayerStatsPresenter>(attribute);
            });
        }
        
        if (Singleton<GameInstance>.Instance.PlayerInstance.HasChildComponent(out Inventory inventory)) {
            this.ToInventoryButton.onClick.AddListener(() => {
                this.Book.PreviousPage();
                this.Book.Open<InventoryCoordinator>(inventory);
            });
        }
        
        this.ResumeButton.onClick.AddListener(this.Book.CloseAll);
        this.RestartButton.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Singleton<GameInstance>.Instance.LoadGame();
        });
        this.QuitButton.onClick.AddListener(Application.Quit);
    }

    public void Present(object model) { }
}
