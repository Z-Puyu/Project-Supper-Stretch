using System;
using CommonFrameworks.Utilities;
using GameManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI {
    public sealed class MainMenuController : UiController {
        private Button StartButton { set; get; }
        private VisualElement OtherButtons { set; get; }
        private Button NewGameButton { set; get; }
        private Button QuitGameButton { set; get; }
        
        private bool IsGameStarted { get; set; }

        protected override void Awake() {
            base.Awake();
            this.StartButton = this.Root.Q<Button>("StartButton");
            this.OtherButtons = this.Root.Q<VisualElement>("Buttons");
            this.NewGameButton = this.OtherButtons.Q<Button>("NewGameButton");
            this.QuitGameButton = this.OtherButtons.Q<Button>("QuitGameButton");
        }

        private void OnEnable() {
            this.QuitGameButton.clicked += Application.Quit;
            this.NewGameButton.clicked += Singleton<GameInstance>.Instance.StartNewGame;
        }

        private void OnDisable() {
            this.QuitGameButton.clicked -= Application.Quit;
            this.NewGameButton.clicked -= Singleton<GameInstance>.Instance.StartNewGame;
        }

        private void Update() {
            if (!Input.anyKeyDown || this.IsGameStarted) {
                return;
            }

            this.StartButton.style.display = DisplayStyle.None;
            this.OtherButtons.style.display = DisplayStyle.Flex;
            this.IsGameStarted = true;
        }
    }
}