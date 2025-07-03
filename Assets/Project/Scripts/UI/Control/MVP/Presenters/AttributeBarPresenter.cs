using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.Common.GameplayTags;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public class AttributeBarPresenter : ProgressBarPresenter<AttributeSet> {
    private AdvancedDropdownList<string> Attributes => GameplayTagTree<AttributeType>.Instances
                                                                                     .OfType<AttributeDefinition>()
                                                                                     .LeafTags();
    
    [field: SerializeField, AdvancedDropdown(nameof(this.Attributes))] 
    protected string Attribute { get; private set; } = string.Empty;
    
    [field: SerializeField] private AttributeSet? Source { get; set; }

    private void Start() {
        if (!this.Source) {
            return;
        }

        this.UpdateView(this.Source);
    }

    private void Parse(AttributeChange data) {
        if (data.Type != this.Attribute) {
            return;
        }

        if (this.Source) {
            this.View.MaxValue = this.Source.ReadMax(this.Attribute);
        }

        this.View.CurrentValue = data.NewCurrentValue;
        this.View.Refresh();
    }

    protected override void UpdateView(AttributeSet model) {
        if (this.Source) {
            this.Source.OnAttributeChanged -= this.Parse;
        }

        this.Source = model;
        this.Source.OnAttributeChanged += this.Parse;
        this.View.CurrentValue = this.Source.ReadCurrent(this.Attribute);
        this.View.MaxValue = this.Source.ReadMax(this.Attribute);
        this.View.Refresh();
    }
}
