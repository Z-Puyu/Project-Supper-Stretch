using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common;
using Project.Scripts.UI.Control.MVP;
using Project.Scripts.UI.Control.MVP.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent, RequireComponent(typeof(Image), typeof(CanvasGroup))]
public class UIPage : MonoBehaviour {
    public static event UnityAction OnActivated = delegate { };
    public static event UnityAction OnDeactivated = delegate { };
    
    [NotNull] public CanvasGroup? CanvasGroup { get; private set; }
    [NotNull] public IPresenter? MainPresenter { get; private set; }
    public bool IsOpen { get; private set; }
    public bool IsClosed => !this.IsOpen;

    private void Awake() {
        this.CanvasGroup = this.GetComponent<CanvasGroup>();
        this.MainPresenter = this.GetComponentInChildren<IPresenter>(includeInactive: true);
    }

    private void Start() {
        this.gameObject.SetActive(false);
    }

    public void Open() {
        this.IsOpen = true;
        this.gameObject.SetActive(true);
        Time.timeScale = 0;
        this.CanvasGroup.blocksRaycasts = true;
        UIPage.OnActivated.Invoke();
    }

    public void Close() {
        this.CanvasGroup.blocksRaycasts = false;
        this.IsOpen = false;
        this.gameObject.SetActive(false);
        Time.timeScale = 1;
        UIPage.OnDeactivated.Invoke();
    }

    public void Refresh(IPresentable data) {
        this.MainPresenter.Present(data);   
    }
}
