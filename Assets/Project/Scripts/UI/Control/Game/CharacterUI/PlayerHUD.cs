using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.Characters.Player;
using Project.Scripts.GameManagement;
using Project.Scripts.Util.Singleton;
using UnityEngine;

namespace Project.Scripts.UI.Control.Game.CharacterUI;

public class PlayerHUD : MonoBehaviour {
    private PlayerCharacter? Player { get; set; }
    [field: SerializeField] private List<AttributeBarPresenter> AttributeBars { get; set; } = [];

    private void Start() {
        this.Player = Singleton<GameInstance>.Instance.PlayerInstance;
        AttributeSet attributeSet = this.Player.GetComponent<AttributeSet>();
        this.AttributeBars.ForEach(bar => bar.Present(attributeSet));
        attributeSet.OnAttributeChanged += handleAttributeChange;
        return;

        void handleAttributeChange(AttributeChange change) {
            this.AttributeBars.ForEach(bar => bar.Present(change));
        }
    }
}
