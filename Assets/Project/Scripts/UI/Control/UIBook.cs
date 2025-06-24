using System;
using System.Collections.Generic;
using Project.Scripts.Common;
using Project.Scripts.Common.Input;
using Project.Scripts.Player;
using Project.Scripts.UI.Control.MVP;
using Project.Scripts.UI.Control.MVP.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent]
public class UIBook : MonoBehaviour, IUserInterface {
    private Dictionary<Type, UIPage> Pages { get; init; } = [];
    private Stack<UIPage> History { get; init; } = [];

    private void Start() {
        foreach (UIPage page in this.GetComponentsInChildren<UIPage>(includeInactive: true)) {
            this.AddNewPage(page);
        }
    }

    public void AddNewPage(UIPage page) {
        if (this.Pages.ContainsValue(page)) {
            Debug.LogWarning($"Page {page} already exists.");
            return;
        }

        if (page.transform.parent != this.transform) {
            page.transform.SetParent(this.transform);   
        }
        
        this.Pages.Add(page.MainPresenter.GetType(), page);
    }

    /// <summary>
    /// Refresh a page with new data. Usually, the page should be open when this method is called.
    /// </summary>
    /// <param name="data">The data to display on the page.</param>
    /// <typeparam name="U">The presenter's type.</typeparam>
    public void Refresh<U>(IPresentable data) where U : IPresenter {
        if (!this.Pages.TryGetValue(typeof(U), out UIPage page)) {
            Debug.LogError($"No page found for UI component {typeof(U)}");
            return;
        }
        
        page.Refresh(data);
    }
    
    /// <summary>
    /// Opens a page with initial data.
    /// </summary>
    /// <param name="data">The initial data to display when the page opens.</param>
    /// <typeparam name="U">The presenter's type.</typeparam>
    public void Open<U>(IPresentable data) where U : IPresenter {
        if (!this.Pages.TryGetValue(typeof(U), out UIPage page)) {
            Debug.LogError($"No page found for UI component {typeof(U)}");
            return;
        }

        if (page.IsOpen) {
            Debug.LogWarning($"Page {page} is already open.");
            return;
        }
        
        page.Open();
        page.Refresh(data);
        this.History.Push(page);
        if (this.History.Count == 1) {
            GameEvents.OnPause.Invoke();
        }
    }

    public void PreviousPage() {
        while (this.History.TryPeek(out UIPage page) && page.IsClosed) {
            this.History.Pop();
        }

        if (!this.History.TryPop(out UIPage top)) {
            return;
        }

        top.Close();
        while (this.History.TryPeek(out UIPage prev) && prev.IsClosed) {
            this.History.Pop();
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
            if (page.IsOpen) {
                page.Close();   
            }
        }
        
        GameEvents.OnPlay.Invoke();
    }

    public void BindInput(InputActions actions) {
        actions.UI.GoBack.performed += _ => this.PreviousPage();
    }
}
