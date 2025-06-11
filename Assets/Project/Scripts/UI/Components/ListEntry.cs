using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.UI.Components.Styles.Themes;
using Project.Scripts.Util.Pooling;
using UnityEngine.UI;

namespace Project.Scripts.UI.Components;

public abstract class ListEntry : Button {
    [NotNull] private HorizontalOrVerticalLayoutGroup? LayoutGroup { get; set; }
    
    protected override void Setup() {
        base.Setup();
        this.LayoutGroup = this.GetComponent<HorizontalOrVerticalLayoutGroup>();
    }
    
    protected override void ApplyTheme(Theme theme) { }
    
    protected override void RevertTheme() { }

    public virtual void OnRemove() {
        this.RemoveEventListeners();
    }
}

public abstract class ListEntry<L> : ListEntry, IPoolable<ListEntry<L>> {
    public event Action<ListEntry<L>> OnReturn = delegate { };

    public override void OnRemove() {
        base.OnRemove();
        this.OnReturn.Invoke(this);
        this.OnReturn = delegate { };
    }
}
