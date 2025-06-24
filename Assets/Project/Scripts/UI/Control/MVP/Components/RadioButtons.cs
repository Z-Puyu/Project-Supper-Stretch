using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control.MVP.Components;

public class RadioButtons : ToggleGroup {
    private List<Toggle> Options { get; init; } = [];
    private List<Toggle> CurrentSelection { get; set; } = [];
    private bool IsUpdating { get; set; }

    [field: SerializeField] private List<Toggle> Default { get; set; } = [];
    [field: SerializeField] public bool AllowMultipleSelection { get; private set; }
    
    public event UnityAction<int> OnSelected = delegate { };
    public event UnityAction<int> OnDeselected = delegate { };

    protected override void Awake() {
        base.Awake();
        this.GetComponentsInChildren(this.Options);
    }

    protected override void Start() {
        for (int i = 0; i < this.Options.Count; i += 1) {
            int index = i;
            this.Options[i].isOn = false;
            this.Options[i].group = this;
            this.Options[i].onValueChanged.AddListener(toggled => this.OnButtonToggled(index, toggled));
        }

        for (int i = this.Default.Count - 1; i >= 0; i -= 1) {
            this.Default[i].isOn = true;
        }
    }

    private void OnButtonToggled(int index, bool toggled) {
        if (this.IsUpdating) {
            return;
        }

        this.IsUpdating = true;
        if (!this.AllowMultipleSelection && toggled) {
            this.CurrentSelection.ForEach(toggle => toggle.isOn = false);
            for (int i = 0; i < this.CurrentSelection.Count; i += 1) {
                this.OnDeselected.Invoke(i);
            }
            
            this.CurrentSelection.Clear(); 
        }
        
        if (toggled) {
            this.CurrentSelection.Add(this.Options[index]);
            this.OnSelected.Invoke(index);
        } else {
            this.CurrentSelection.Remove(this.Options[index]);
            this.OnDeselected.Invoke(index);
        }
        
        this.IsUpdating = false;
    }
}
