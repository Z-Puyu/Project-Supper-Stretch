using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.UI.Components.Styles;
using Project.Scripts.UI.Components.Styles.Themes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class ProgressBar : UIComponent<ProgressBarStyle> {
    private enum Section { Main, SecondaryOnIncrease, SecondaryOnDecrease }

    [NotNull]
    [field: SerializeField]
    private Image? Background { get; set; }

    private List<Image> FillSections { get; init; } = [];
    private List<Slider> SliderSections { get; init; } = [];

    private (Slider slider, Image fill) this[Section section] =>
            (this.SliderSections[(int)section], this.FillSections[(int)section]);

    protected override void Setup() {
        this.GetComponentsInChildren(includeInactive: true, this.SliderSections);
        this.SliderSections.ForEach(slider => this.FillSections.Add(slider.fillRect.GetComponent<Image>()));
    }

    protected override void ApplyTheme(Theme theme) {
        this[Section.SecondaryOnDecrease].fill.color = theme.SpriteColour(UIStyleUsage.NegativeIndication);
        this[Section.SecondaryOnIncrease].fill.color = theme.SpriteColour(UIStyleUsage.PositiveIndication);
    }

    protected override void RevertTheme() {
        this[Section.SecondaryOnDecrease].fill.color = Color.white;
        this[Section.SecondaryOnIncrease].fill.color = Color.white;
    }

    protected override void ApplyStyle(ProgressBarStyle style) {
        this.Background.color = style.BackgroundColour;
        this[Section.Main].fill.color = style.FillColour;
        this[Section.SecondaryOnDecrease].slider.gameObject.SetActive(style.HasSecondaryFill);
        this[Section.SecondaryOnIncrease].slider.gameObject.SetActive(style.HasSecondaryFill);
        if (!style.HasSecondaryFill || !style.OverrideSecondaryFillColour) {
            return;
        }

        this[Section.SecondaryOnDecrease].fill.color = style.SecondaryFillColourOnDecrease;
        this[Section.SecondaryOnIncrease].fill.color = style.SecondaryFillColourOnIncrease;
    }

    protected override void RevertStyle() {
        this.Background.color = Color.gray;
        this[Section.Main].fill.color = Color.white;
        this[Section.SecondaryOnDecrease].fill.color = Color.white;
        this[Section.SecondaryOnIncrease].fill.color = Color.white;
        this[Section.SecondaryOnDecrease].slider.gameObject.SetActive(false);
        this[Section.SecondaryOnIncrease].slider.gameObject.SetActive(false);
    }

    private void SetCurrentAndMaxValue(float value, float max) {
        this[Section.Main].slider.maxValue = max;
        this[Section.SecondaryOnDecrease].slider.maxValue = max;
        this[Section.SecondaryOnIncrease].slider.maxValue = max;
        this[Section.Main].slider.value = value;
        this[Section.SecondaryOnDecrease].slider.value = value;
        this[Section.SecondaryOnIncrease].slider.value = value;
    }
    
    private void SetCurrentValue(float value) {
        this[Section.Main].slider.value = value;
        this[Section.SecondaryOnDecrease].slider.value = value;
        this[Section.SecondaryOnIncrease].slider.value = value;
    }

    public override void Display(object data) {
        try {
            if (data is ({ } curr, { } max)) {
                this.SetCurrentAndMaxValue(parse(curr), parse(max));
            } else {
                this.SetCurrentValue(parse(data));
            }
        } catch (Exception) {
            Debug.LogWarning($"Invalid data {data} passed to ProgressBar.");
        }

        return;

        float parse(object value) => value switch {
            int @int => @int,
            float @float => @float,
            string @string when float.TryParse(@string, out float floatValue) => floatValue,
            var _ => Convert.ToSingle(value)
        };
    }
}
