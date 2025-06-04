using System;
using System.Collections.Generic;
using Project.Scripts.UI.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.UI.Control;

public class UIPage : MonoBehaviour {
    public static event UnityAction OnActivated = delegate { };
    public static event UnityAction OnDeactivated = delegate { };
    
    private Dictionary<Type, UIPresenter> Presenters { get; init; } = [];
    public bool IsOpen { get; private set; }
    public bool IsClosed => !this.IsOpen;
    public IEnumerable<UIPresenter> UIComponents => this.Presenters.Values;

    private void Awake() {
        foreach (UIPresenter presenter in this.GetComponentsInChildren<UIPresenter>(includeInactive: true)) {
            this.Presenters.Add(presenter.GetType(), presenter);
        }
    }

    public void Open() {
        this.IsOpen = true;
        this.gameObject.SetActive(true);
        Time.timeScale = 0;
        UIPage.OnActivated.Invoke();
    }

    public void Close() {
        this.IsOpen = false;
        this.gameObject.SetActive(false);
        Time.timeScale = 1;
        UIPage.OnDeactivated.Invoke();
    }

    public void Refresh<U>(object? data = null) where U : UIPresenter {
        if (data is null) {
            this.Presenters[typeof(U)].Present();
        } else {
            this.Presenters[typeof(U)].Present(data);
        }
    }
}
