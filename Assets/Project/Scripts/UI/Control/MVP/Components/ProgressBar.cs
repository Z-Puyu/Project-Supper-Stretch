using System.Collections.Generic;
using UnityEngine.UI;

namespace Project.Scripts.UI.Control.MVP.Components;

public class ProgressBar : UIView {
    private enum Section { Main, SecondaryOnIncrease, SecondaryOnDecrease }

    public float CurrentValue { private get; set; }
    public float MaxValue { private get; set; }
    
    private List<Image> FillSections { get; init; } = [];
    private List<Slider> SliderSections { get; init; } = [];

    private (Slider slider, Image fill) this[Section section] =>
            (this.SliderSections[(int)section], this.FillSections[(int)section]);

    private void Awake() {
        this.GetComponentsInChildren(includeInactive: true, this.SliderSections);
        this.SliderSections.ForEach(slider => this.FillSections.Add(slider.fillRect.GetComponent<Image>()));
    }

    private void SetCurrentAndMaxValue(float value, float max) {
        this[Section.Main].slider.maxValue = max;
        this[Section.SecondaryOnDecrease].slider.maxValue = max;
        this[Section.SecondaryOnIncrease].slider.maxValue = max;
        this[Section.Main].slider.value = value;
        this[Section.SecondaryOnDecrease].slider.value = value;
        this[Section.SecondaryOnIncrease].slider.value = value;
    }

    public override void Refresh() {
        this.SetCurrentAndMaxValue(this.CurrentValue, this.MaxValue);
    }
    
    public override void Clear() {
        this.CurrentValue = 0;
        this.MaxValue = 0;
    }
}
