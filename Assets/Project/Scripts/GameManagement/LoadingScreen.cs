using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

namespace Project.Scripts.GameManagement;

[DisallowMultipleComponent, RequireComponent(typeof(Canvas))]
public class LoadingScreen : MonoBehaviour {
    [NotNull] [field: SerializeField] private TextMeshProUGUI? HintText { get; set; }
    [NotNull] private CanvasGroup? CanvasGroup { get; set; }

    private void Awake() {
        this.CanvasGroup = this.HintText.GetComponent<CanvasGroup>();
    }

    private void OnDisable() {
        LeanTween.cancel(this.HintText.gameObject);
    }

    public void FlashHintText(string text) {
        LeanTween.cancel(this.HintText.gameObject);
        LeanTween.alphaCanvas(this.CanvasGroup, 0, 0.5f).setLoopPingPong(-1);
        this.HintText.text = text;
    }
}
