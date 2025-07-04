using System;
using System.Collections.Generic;
using Project.Scripts.Common;
using Project.Scripts.Common.Input;
using Project.Scripts.GameManagement;
using Project.Scripts.UI.Control.MVP;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent]
public class UIBook : MonoBehaviour, IUserInterface {
    private Dictionary<Type, UIPage> Pages { get; init; } = [];
    private Stack<UIPage> History { get; init; } = [];
    [field: SerializeField] private UserInterfaceAudio? Audio { get; set; }

    private void Start() {
        Singleton<GameInstance>.Instance.PlayerInstance.OnKilled += this.CloseAll;
        foreach (UIPage page in this.GetComponentsInChildren<UIPage>(includeInactive: true)) {
            this.AddNewPage(page);
        }
    }

    public void AddNewPage(UIPage page) {
        if (this.Pages.ContainsValue(page)) {
            Logging.Warn($"Page {page} already exists.", this);
            return;
        }

        if (page.transform.parent != this.transform) {
            page.transform.SetParent(this.transform);   
        }
        
        this.Pages.Add(page.MainPresenter.GetType(), page);
        page.gameObject.SetActive(false);
    }

    /// <summary>
    /// Refresh a page with new data. Usually, the page should be open when this method is called.
    /// </summary>
    /// <param name="data">The data to display on the page.</param>
    /// <typeparam name="U">The presenter's type.</typeparam>
    public void Refresh<U>(IPresentable data) where U : IPresenter {
        if (!this.Pages.TryGetValue(typeof(U), out UIPage page)) {
            Logging.Error($"No page found for UI component {typeof(U)}", this);
            return;
        }
        
        page.Refresh(data);
    }
    
    /// <summary>
    /// Opens a page with initial data.
    /// </summary>
    /// <param name="data">The initial data to display when the page opens.</param>
    /// <typeparam name="U">The presenter's type.</typeparam>
    public void Open<U>(IPresentable? data = null) where U : IPresenter {
        if (!this.Pages.TryGetValue(typeof(U), out UIPage page)) {
            Logging.Error($"No page found for UI component {typeof(U)}", this);
            return;
        }

        if (page.IsOpen) {
            Debug.LogWarning($"Page {page} is already open.");
            return;
        }
        
        page.Canvas.sortingOrder = this.History.Count;
        page.Open();
        if (data is not null) {
            page.Refresh(data);
        }
        
        this.History.Push(page);
        if (this.Audio) {
            this.Audio.Play(UserInterfaceAudio.Sound.Enable);
        }
        
        if (this.History.Count == 1) {
            GameEvents.OnPause.Invoke();
        }
    }

    public void PreviousPage() {
        Singleton<TooltipSystem>.Instance.Hide();
        while (this.History.TryPeek(out UIPage page) && page.IsClosed) {
            this.History.Pop();
        }

        if (!this.History.TryPop(out UIPage top)) {
            return;
        }

        if (!top.CanBeClosed) {
            return;
        }
        
        top.Close();
        while (this.History.TryPeek(out UIPage prev) && prev.IsClosed) {
            this.History.Pop();
        }
        
        if (this.Audio) {
            this.Audio.Play(UserInterfaceAudio.Sound.Disable);
        }

        if (this.History.Count == 0) {
            GameEvents.OnPlay.Invoke();
        }
    }

    public void Toggle<U>(IPresentable data) where U : IPresenter {
        if (!this.Pages.TryGetValue(typeof(U), out UIPage page)) {
            Debug.LogError($"No page found for UI component {typeof(U)}");
        } else if (page.IsClosed) {
            this.Open<U>(data);
        } else if (this.History.Peek() == page) {
            this.PreviousPage();
        }
    }
    
    public void CloseAll() {
        while (this.History.TryPop(out UIPage page)) {
            if (!page.IsOpen) {
                continue;
            }

            page.Close();
            if (this.Audio) {
                this.Audio.Play(UserInterfaceAudio.Sound.Disable);
            }
        }
        
        GameEvents.OnPlay.Invoke();
    }

    public void BindInput(InputActions actions) {
        actions.UI.GoBack.performed += _ => this.PreviousPage();
    }
}
