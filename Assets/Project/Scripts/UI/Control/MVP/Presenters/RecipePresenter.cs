using Project.Scripts.Items.CraftingSystem;
using Project.Scripts.UI.Control.MVP.Components;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public class RecipePresenter : UIPresenter<Recipe, TextView> {
    protected override void UpdateView(Recipe model) {
        this.View.Content = model.FormatAsText();
        this.View.Refresh();
    }
}
