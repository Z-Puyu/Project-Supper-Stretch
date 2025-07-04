using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Project.Scripts.Common;
using Project.Scripts.Util.Linq;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Components;

public class Tabs : MonoBehaviour {
    [NotNull]
    [field: SerializeField, Required]
    private RadioButtons? TabSelector { get; set; }
    
    [field: SerializeField] private GameObject? CurrentTab { get; set; }
    private List<GameObject> AllTabs { get; set; } = [];

    private void Awake() {
        foreach (Transform child in this.transform) {
            this.AllTabs.Add(child.gameObject);
        }
    }

    private void Start() {
        if (this.TabSelector.AllowMultipleSelection) {
            Logging.Warn("Should not allow multiple selection for tabs.", this);
        }
        
        this.TabSelector.OnSelected += this.GoTo;
        this.TabSelector.OnDeselected += this.Close;
        this.TabSelector.gameObject.SetActive(true);
        this.AllTabs.Where(tab => tab != this.CurrentTab).ForEach(tab => tab.SetActive(false));
    }

    private void Close(int tab) {
        if (this.AllTabs[tab] != this.CurrentTab) {
            Logging.Warn("Trying to close a tab that is not the current tab.", this);
        } else {
            this.CurrentTab.SetActive(false);
            this.CurrentTab = null;
        }
    }

    private void GoTo(int tab) {
        if (this.AllTabs.Count <= tab) {
            Logging.Warn("Trying to go to a tab that does not exist.", this);
        } else {
            if (this.CurrentTab) {
                this.CurrentTab.SetActive(false);
            }
            
            this.CurrentTab = this.AllTabs[tab];
            this.CurrentTab.SetActive(true);
            this.CurrentTab.GetComponentsInChildren<IPresenter>().ForEach(presenter => presenter.Refresh());
        }
    }
}
