using System;
using Editor;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Control.Game.CharacterUI;

public class AttributeBarPresenter : ProgressBarPresenter<AttributeSet, AttributeChange> {
    private AdvancedDropdownList<AttributeKey> Attributes =>
            ObjectCache<AttributeDefinition>.Instance.Objects.FetchOnlyLeaves();
    
    [field: SerializeField, AdvancedDropdown(nameof(this.Attributes))] 
    protected AttributeKey Attribute { get; private set; }

    private void Start() {
        if (!this.Model) {
            return;
        }

        this.Model.OnAttributeChanged += this.Present;
        this.Refresh();
    }

    public override void Present(AttributeChange data) {
        if (data.Type != this.Attribute) {
            return;
        }

        if (!this.Model) {
            this.View.Display(data.NewCurrentValue);
        } else {
            this.View.Display((data.NewCurrentValue, this.Model.ReadMax(this.Attribute.FullName)));
        }
    }
    
    public override void Refresh() {
        if (!this.Model) {
            return;
        }
        
        string key = this.Attribute.FullName;
        this.View.Display((this.Model.ReadCurrent(key), this.Model.ReadMax(key)));
    }
}
