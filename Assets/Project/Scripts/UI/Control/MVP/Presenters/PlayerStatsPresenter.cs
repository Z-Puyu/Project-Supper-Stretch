using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.UI.Control.MVP.Components;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public class PlayerStatsPresenter : UIPresenter<AttributeSet, PlayerStatsView> {
    protected override void UpdateView(AttributeSet model) {
        this.View.PlayerAttribute = model.FormatAsText();
        this.View.Refresh();   
    }
}
