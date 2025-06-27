using Project.Scripts.Items;
using Project.Scripts.Items.CraftingSystem;
using Project.Scripts.UI.Control.MVP.Components;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public class RecipePresenter : UIPresenter<CampFire.UIData, TextView> {
    protected override void UpdateView(CampFire.UIData model) {
        this.View.Content = model.Workbench.TryProduce(out Item item) ? item.FormatAsText() : string.Empty;
        this.View.Refresh();
    }
}
