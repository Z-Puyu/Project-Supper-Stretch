using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Characters.Player;
using Project.Scripts.UI.Components;
using UnityEngine;

namespace Project.Scripts.UI.Control.Game.CharacterUI;

public class ExperienceBarPresenter : ProgressBarPresenter<ExperienceSystem, ExperienceSystem.UIData> {
    [NotNull] [field: SerializeField] private Text? LevelText { get; set; }

    public override void Present(ExperienceSystem.UIData data) {
        this.LevelText.Display(data.Value.level);
        this.View.Display((data.Value.xp, data.Value.xpToNextLevel));
    }

    public override void Refresh() {
        if (!this.Model) {
            return;
        }
            
        this.View.Display((this.Model.CurrentXp, this.Model.XpToNextLevel));
        this.LevelText.Display(this.Model.CurrentLevel);
    }
}
