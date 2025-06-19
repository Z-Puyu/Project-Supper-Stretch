using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Flexalon.Runtime.Core;
using Project.Scripts.UI.Styles;
using Project.Scripts.UI.Styles.Themes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public class ProgressBar : UIComponent<ProgressBarStyle> {
    private enum Section { Main, SecondaryOnIncrease, SecondaryOnDecrease }

    [NotNull]
    [field: SerializeField]
    private Image? Background { get; set; }

    [NotNull]
    [field: SerializeField]
    private Outline? Outline { get; set; }
    
    [NotNull]
    [field: SerializeField]
    private FlexalonObject? Flex { get; set; }

    private List<Image> FillSections { get; init; } = [];
    private List<Slider> SliderSections { get; init; } = [];

    private (Slider slider, Image fill) this[Section section] =>
            (this.SliderSections[(int)section], this.FillSections[(int)section]);

    protected override void Setup() {
        this.GetComponentsInChildren(includeInactive: true, this.SliderSections);
        this.SliderSections.ForEach(slider => this.FillSections.Add(slider.fillRect.GetComponent<Image>()));
    }

    protected override void ApplyTheme(Theme theme) {
        this.Outline.effectColor = theme.SeparatorColour;
        this[Section.SecondaryOnDecrease].fill.color = theme.SpriteColour(UIStyleUsage.NegativeIndication);
        this[Section.SecondaryOnIncrease].fill.color = theme.SpriteColour(UIStyleUsage.PositiveIndication);
    }

    protected override void RevertTheme() {
        this.Outline.effectColor = Color.clear;
        this.Background.color = Color.black;
        this[Section.SecondaryOnDecrease].fill.color = Color.white;
        this[Section.SecondaryOnIncrease].fill.color = Color.white;
    }

    protected override void ApplyStyle(ProgressBarStyle style) {
        this.Outline.effectDistance = style.BorderWidth;
        this.Outline.effectColor = style.BorderColour;
        this.Flex.PaddingBottom = this.Flex.PaddingTop = style.BorderWidth.y;
        this.Flex.PaddingLeft = this.Flex.PaddingRight = style.BorderWidth.x;
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
        this.Background.color = Color.black;
        this[Section.Main].fill.color = Color.white;
        this[Section.SecondaryOnDecrease].fill.color = Color.white;
        this[Section.SecondaryOnIncrease].fill.color = Color.white;
        this[Section.SecondaryOnDecrease].slider.gameObject.SetActive(false);
        this[Section.SecondaryOnIncrease].slider.gameObject.SetActive(false);
        this.Outline.effectDistance = Vector2.zero;
        this.Outline.effectColor = Color.clear;
        this.Flex.Padding = Directions.zero;
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

    public override void Display(object? data) {
        if (data is null) {
            return;
        }
        
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
