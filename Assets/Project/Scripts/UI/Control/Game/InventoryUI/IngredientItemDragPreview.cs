using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Items;
using Project.Scripts.UI.Components;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Control.Game.InventoryUI;

public class IngredientItemDragPreview : DragPreview {
    [NotNull] 
    [field: SerializeField, Required]
    private Text? Nameplate { get; set; }
    
    public override void Configure() {
        if (this.Payload is Item item) {
            this.Nameplate.Display(item.Name);
        }
    }
}
