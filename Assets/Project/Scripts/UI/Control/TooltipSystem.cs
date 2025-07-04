using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Common;
using Project.Scripts.Util.Singleton;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.UI.Control;

public class TooltipSystem : Singleton<TooltipSystem> {
    [NotNull]
    [field: SerializeField, Required] 
    private Tooltip? Tooltip { get; set; }

    private void Start() {
        GameEvents.UI.OnGoBack += this.Hide;
    }

    public void Show(string content, string header = "") {
        this.Tooltip.gameObject.SetActive(true);
        this.Tooltip.SetText(content, header);
    }
    
    public void Hide() {
        this.Tooltip.gameObject.SetActive(false);
    }
}
