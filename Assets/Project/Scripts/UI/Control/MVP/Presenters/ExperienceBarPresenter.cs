using System;
using Project.Scripts.Characters.Player;
using Project.Scripts.Common;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public class ExperienceBarPresenter : ProgressBarPresenter<ExperienceSystem> {
    private void Start() {
        ExperienceSystem.OnExperienceChanged += this.UpdateView;
    }

    protected override void UpdateView(ExperienceSystem model) {
        this.View.CurrentValue = model.CurrentXp;
        this.View.MaxValue = model.XpToNextLevel;
        this.View.ValueLabelText = model.CurrentLevel.ToString();
        this.View.Refresh();  
    }

    private void OnDestroy() {
        ExperienceSystem.OnExperienceChanged -= this.UpdateView;
    }
}
