using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control.MVP.Components;

public class ProgressBar : UIView {
    private enum Section { Main, SecondaryOnIncrease, SecondaryOnDecrease }
    
    [field: SerializeField] private TextMeshProUGUI? ChangeIndicator { get; set; }
    [field: SerializeField] private TextMeshProUGUI? ValueLabel { get; set; }

    public float CurrentValue { private get; set; }
    public float MaxValue { private get; set; }
    public string ValueLabelText { private get; set; } = string.Empty;
    
    private List<Image> FillSections { get; init; } = [];
    private List<Slider> SliderSections { get; init; } = [];
    private Vector3 TextOrigin { get; set; }
    
    private (Slider slider, Image fill) this[Section section] =>
            (this.SliderSections[(int)section], this.FillSections[(int)section]);

    private void Awake() {
        this.GetComponentsInChildren(includeInactive: true, this.SliderSections);
        this.SliderSections.ForEach(slider => this.FillSections.Add(slider.fillRect.GetComponent<Image>()));
    }

    private void Start() {
        if (this.ChangeIndicator) {
            this.TextOrigin = this.ChangeIndicator.transform.localPosition;
            //this.ChangeIndicator.text = string.Empty;
        }
    }

    private void SetCurrentAndMaxValue(float value, float max) {
        if (Math.Abs(this[Section.Main].slider.maxValue - max) >= 1) {
            this[Section.Main].slider.maxValue = max;
            this[Section.SecondaryOnDecrease].slider.maxValue = max;
            this[Section.SecondaryOnIncrease].slider.maxValue = max;
        }

        if (Math.Abs(this[Section.Main].slider.value - value) < 1) {
            return;
        }

        this[Section.Main].slider.value = value;
        this[Section.SecondaryOnDecrease].slider.value = value;
        this[Section.SecondaryOnIncrease].slider.value = value;
    }

    public override void Refresh() {
        if (this.ChangeIndicator) {
            int change = Mathf.FloorToInt(this.CurrentValue - this[Section.Main].slider.value);
            if (change != 0) {
                float textHeight = this.ChangeIndicator.bounds.extents.y * 2;
                this.ChangeIndicator.transform.localScale = Vector3.zero;
                this.ChangeIndicator.color = Color.clear;
                Color targetColor = change > 0
                        ? this.FillSections[(int)Section.SecondaryOnIncrease].color
                        : this.FillSections[(int)Section.SecondaryOnDecrease].color;
                this.ChangeIndicator.text = $"{change:+#;-#;#}";
                LeanTween.scale(this.ChangeIndicator.gameObject, Vector3.one, 0.5f);
                LeanTween.moveLocalY(this.ChangeIndicator.gameObject, this.TextOrigin.y + textHeight, 0.5f);
                LeanTween.value(this.ChangeIndicator.gameObject, color => this.ChangeIndicator.color = color,
                    this.ChangeIndicator.color, targetColor, 0.5f).setLoopPingPong(1).setOnComplete(() =>
                        this.ChangeIndicator.transform.localPosition = this.TextOrigin);
            }   
        }
        
        this.SetCurrentAndMaxValue(this.CurrentValue, this.MaxValue);
        if (this.ValueLabel) {
            this.ValueLabel.text = this.ValueLabelText == string.Empty
                    ? $"{this.CurrentValue}/{this.MaxValue}"
                    : this.ValueLabelText;
        }
    }
    
    public override void Clear() {
        this.CurrentValue = 0;
        this.MaxValue = 0;
    }
}
