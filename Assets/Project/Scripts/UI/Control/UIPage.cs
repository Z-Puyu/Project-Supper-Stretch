using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common;
using Project.Scripts.UI.Control.MVP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent, RequireComponent(typeof(Image), typeof(CanvasGroup), typeof(Canvas))]
public class UIPage : MonoBehaviour {
    [NotNull] public Canvas? Canvas { get; private set; }
    [NotNull] public CanvasGroup? CanvasGroup { get; private set; }
    [NotNull] public IPresenter? MainPresenter { get; private set; }
    [field: SerializeField] public bool CanBeClosed { get; private set; } = true;
    public bool IsOpen { get; private set; }
    public bool IsClosed => !this.IsOpen;

    private void Awake() {
        this.Canvas = this.GetComponent<Canvas>();
        this.CanvasGroup = this.GetComponent<CanvasGroup>();
        this.MainPresenter = this.GetComponentInChildren<IPresenter>(includeInactive: true);
    }

    public void Open() {
        this.IsOpen = true;
        this.gameObject.SetActive(true);
        //Time.timeScale = 0;
        this.CanvasGroup.blocksRaycasts = true;
    }

    public void Close() {
        this.CanvasGroup.blocksRaycasts = false;
        this.IsOpen = false;
        this.gameObject.SetActive(false);
        //Time.timeScale = 1;
    }

    public void Refresh(IPresentable data) {
        this.MainPresenter.Present(data);   
    }
}
