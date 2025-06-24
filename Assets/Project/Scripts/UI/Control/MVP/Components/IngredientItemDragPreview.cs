using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Items;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Components;

public class IngredientItemDragPreview : DragPreview {
    [NotNull] 
    [field: SerializeField, Required]
    private TextView? Nameplate { get; set; }
    
    public override void Configure() {
        if (this.Payload is not Item item) {
            return;
        }

        this.Nameplate.Content = item.Name;
        this.Nameplate.Refresh();
    }
}
