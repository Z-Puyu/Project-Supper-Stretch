using System.Collections.Generic;
using Project.Scripts.UI.Styles;
using Project.Scripts.UI.Styles.Themes;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class RadioButtons : UIComponent<RadioButtonsStyle> {
    private ToggleGroup? ToggleGroup { get; set; }
    private List<Toggle> Options { get; init; } = [];
    
    private bool AllowMultipleSelection { get; set; }
    private bool AllowDeselect { get; set; }
    
    public event UnityAction<int> OnSelect = delegate { };
    public event UnityAction<int> OnDeselect = delegate { };
    
    protected override void Setup() {
        this.ToggleGroup = this.GetComponentInChildren<ToggleGroup>();
        this.GetComponentsInChildren(this.Options);
        for (int i = 0; i < this.Options.Count; i += 1) {
            int index = i;
            this.Options[i].group = this.ToggleGroup;
            this.Options[i].onValueChanged.AddListener(toggled => this.OnButtonToggled(index, toggled));
        }
    }

    private void OnButtonToggled(int index, bool toggled) {
        if (toggled) {
            this.OnSelect.Invoke(index);
        } else {
            this.OnDeselect.Invoke(index);
        }
    }
    
    protected override void ApplyTheme(Theme theme) { }
    
    protected override void RevertTheme() { }
    
    protected override void ApplyStyle(RadioButtonsStyle style) {
        this.AllowMultipleSelection = style.AllowMultipleSelection; 
        this.AllowDeselect = style.AllowDeselect;
    }
    
    protected override void RevertStyle() {
        this.AllowMultipleSelection = false;
        this.AllowDeselect = false;
    }
}
