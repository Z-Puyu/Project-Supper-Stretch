using System;
using System.Collections.Generic;
using Project.Scripts.Common;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.UI.Control;

public class UIBook : MonoBehaviour {
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
        
        page.Close();
        foreach (UIPresenter presenter in page.UIComponents) {
            this.Pages.Add(presenter.GetType(), page);
        }
    }

    /// <summary>
    /// Refresh a page with new data. Usually, the page should be open when this method is called.
    /// </summary>
    /// <param name="data">The data to display on the page.</param>
    /// <typeparam name="U">The presenter's type.</typeparam>
    public void Refresh<U>(object? data = null) where U : UIPresenter {
        if (!this.Pages.TryGetValue(typeof(U), out UIPage page)) {
            Debug.LogError($"No page found for UI component {typeof(U)}");
            return;
        }

        page.Refresh<U>(data);
    }
    
    /// <summary>
    /// Opens a page with initial data.
    /// </summary>
    /// <param name="data">The initial data to display when the page opens.</param>
    /// <typeparam name="U">The presenter's type.</typeparam>
    public void Open<U>(object? data = null) where U : UIPresenter {
        if (!this.Pages.TryGetValue(typeof(U), out UIPage page)) {
            Debug.LogError($"No page found for UI component {typeof(U)}");
            return;
        }

        if (page.IsOpen) {
            Debug.LogWarning($"Page {page} is already open.");
            return;
        }
        
        page.Open();
        page.Refresh<U>(data);
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

    public void Toggle<U>(object? data = null) where U : UIPresenter {
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
}
