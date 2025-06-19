using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using Project.Scripts.Items;
using Project.Scripts.UI.Styles.Themes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class ItemEntry : ListEntry {
    private enum Section { ItemType, ItemName }
    
    [NotNull] private HorizontalOrVerticalLayoutGroup? LayoutGroup { get; set; }
    
    [NotNull] 
    [field: SerializeField, Required] 
    private Button? ClickableRegion { get; set; }
    
    [field: SerializeField] private List<Text> Sections { get; set; } = [];
    
    private Text this[Section section] => this.Sections[(int)section];
    
    protected override void Setup() {
        this.LayoutGroup = this.GetComponent<HorizontalOrVerticalLayoutGroup>();
    }

    protected override void Configure() { }

    protected override void ApplyTheme(Theme theme) { }
    
    protected override void RevertTheme() { }

    public override void Display(object? data) {
        switch (data) {
            case (ItemData item, int count):
                this[Section.ItemType].Display(item.Type);
                this[Section.ItemName].Display($"{item.Name} ({count})");
                break;
            case KeyValuePair<ItemData, int> record:
                this[Section.ItemType].Display(record.Key.Type);
                this[Section.ItemName].Display($"{record.Key.Name} ({record.Value})");
                break;
            case (Item item, int count):   
                this[Section.ItemType].Display(item.Type);
                this[Section.ItemName].Display($"{item.Name} ({count})");
                break;
            case KeyValuePair<Item, int> record:
                this[Section.ItemType].Display(record.Key.Type);
                this[Section.ItemName].Display($"{record.Key.Name} ({record.Value})");
                break;
            default:
                Debug.LogWarning($"Invalid data {data} passed to ItemEntry.");
                break;
        }
    }

    public override void OnRemove() {
        this.ClickableRegion.ClearEvents();
        base.OnRemove();
    }

    public override string ToString() {
        return $"{{{this[Section.ItemType]}, {this[Section.ItemName]}}}";
    }
}