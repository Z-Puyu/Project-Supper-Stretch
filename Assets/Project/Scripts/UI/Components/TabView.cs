using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.UI.Components;

public class TabView : View {
    [field: SerializeField]
    private GameObject? CurrentActiveTab { get; set; }
    
    [field: SerializeField]
    private RadioButtons? TabButtons { get; set; }
    
    private List<GameObject> Tabs { get; set; } = [];
    
    protected override void Setup() {
        base.Setup();
        if (!this.TabButtons) {
            this.TabButtons = this.GetComponentInChildren<RadioButtons>(includeInactive: true);
        }
        
        foreach (RectTransform child in this.transform) {
            this.Tabs.Add(child.gameObject);       
        }
        
        this.Tabs.ForEach(tab => tab.SetActive(tab == this.CurrentActiveTab));
        if (this.TabButtons) {
            this.TabButtons.OnSelect += this.SwitchTab;
        }
    }

    private void SwitchTab(int index) {
        if (this.CurrentActiveTab) {
            this.CurrentActiveTab.SetActive(false);       
        }
        
        this.CurrentActiveTab = this.Tabs[Mathf.Min(index, this.Tabs.Count - 1)];
        this.CurrentActiveTab.SetActive(true);
    }
}
