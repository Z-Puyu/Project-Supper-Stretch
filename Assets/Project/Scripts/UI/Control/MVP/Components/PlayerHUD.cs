using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.Characters.Player;
using Project.Scripts.GameManagement;
using Project.Scripts.UI.Control.MVP.Presenters;
using Project.Scripts.Util.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control.MVP.Components;

public class PlayerHUD : MonoBehaviour {
    private PlayerCharacter? Player { get; set; }
    [field: SerializeField] private List<AttributeBarPresenter> AttributeBars { get; set; } = [];
    [NotNull] [field: SerializeField] private Image? HungerIcon { get; set; }
    [NotNull] [field: SerializeField] private Image? PoisonIcon { get; set; }

    private void Start() {
        this.Player = Singleton<GameInstance>.Instance.PlayerInstance;
        AttributeSet attributeSet = this.Player.GetComponentInChildren<AttributeSet>();
        this.AttributeBars.ForEach(bar => bar.Present(attributeSet));
        PhysicalConditions physicalConditions = this.Player.GetComponentInChildren<PhysicalConditions>();
        physicalConditions.OnFoodPoisoned += () => this.PoisonIcon.color = Color.darkGreen;
        physicalConditions.OnPoisonCured += () => this.PoisonIcon.color = Color.clear;
        physicalConditions.OnSatiated += () => this.HungerIcon.color = Color.clear;
        physicalConditions.OnHungry += () => this.HungerIcon.color = Color.orange;
        physicalConditions.OnStarved += () => this.HungerIcon.color = Color.darkRed;
    }
}
