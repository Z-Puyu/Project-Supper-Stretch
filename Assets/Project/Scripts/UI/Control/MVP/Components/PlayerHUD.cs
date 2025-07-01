using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.Characters.Player;
using Project.Scripts.GameManagement;
using Project.Scripts.UI.Control.MVP.Presenters;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.UI.Control.MVP.Components;

public class PlayerHUD : MonoBehaviour {
    private PlayerCharacter? Player { get; set; }
    [field: SerializeField] private List<AttributeBarPresenter> AttributeBars { get; set; } = [];

    private void Start() {
        this.Player = Singleton<GameInstance>.Instance.PlayerInstance;
        AttributeSet attributeSet = this.Player.GetComponentInChildren<AttributeSet>();
        this.AttributeBars.ForEach(bar => bar.Present(attributeSet));
    }
}
