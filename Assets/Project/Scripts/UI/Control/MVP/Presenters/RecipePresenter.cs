using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Items;
using Project.Scripts.Items.CraftingSystem;
using Project.Scripts.UI.Control.MVP.Components;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public class RecipePresenter : UIPresenter<CampFire.UIData, TextView> {
    [NotNull] 
    [field: SerializeField, Required] 
    private ModifierLocalisationMapping? ModifierLocalisation { get; set; }
    
    protected override void UpdateView(CampFire.UIData model) {
        this.View.Content = $"Time: {model.Workbench.Cost} hours\n" + (model.Workbench.TryProduce(out Item item)
                ? item.FormatAsText()
                : string.Empty);
        this.View.Refresh();
    }
}
