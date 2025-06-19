using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent, RequireComponent(typeof(Image))]
public class UIPage : MonoBehaviour {
    public static event UnityAction OnActivated = delegate { };
    public static event UnityAction OnDeactivated = delegate { };
    
    [NotNull] public IPresenter? MainPresenter { get; private set; }
    public bool IsOpen { get; private set; }
    public bool IsClosed => !this.IsOpen;

    private void Awake() {
        this.MainPresenter = this.GetComponentInChildren<IPresenter>();
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

    /*public void Refresh(object? data = null) {
        if (data is null) {
            this.MainPresenter.Refresh();
        } else {
            this.MainPresenter.Present(data);
        }
    }*/

    public void Refresh(IPresentable data) {
        this.MainPresenter.Present(data);   
    }
}
