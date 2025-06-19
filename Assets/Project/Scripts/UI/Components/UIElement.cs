using Project.Scripts.UI.Styles.Themes;
using SaintsField.Playa;
using UnityEngine;

namespace Project.Scripts.UI.Components;

[DisallowMultipleComponent]
public abstract class UIElement : UIObject {
    [field: SerializeField] protected Theme? Theme { get; private set; }

    private void Awake() {
        this.Init();
    }

    /// <summary>
    /// Fetch references to UI elements in the component.
    /// </summary>
    protected abstract void Setup();
    
    /// <summary>
    /// Stylise the component using the theme and the style.
    /// </summary>
    protected abstract void Configure();
    
    protected abstract void ApplyTheme(Theme theme);

    protected abstract void RevertTheme();

    [Button("Reconfigure Component")]
    public void Init() {
        this.Setup();
        this.ConfigureChildElements();
        this.Configure();
    }

    private void ConfigureChildElements() {
        if (!this.Theme) {
            return;
        }
        
        UIElement[] components = this.GetComponentsInChildren<UIElement>();
        for (int i = components.Length - 1; i >= 0; i -= 1) {
            if (components[i].Theme == this.Theme) {
                continue;
            }

            components[i].Theme = this.Theme;
            components[i].Setup();
            components[i].Configure();
        }
    }
    
    protected virtual void OnValidate() {
        this.Init();
    }
}
