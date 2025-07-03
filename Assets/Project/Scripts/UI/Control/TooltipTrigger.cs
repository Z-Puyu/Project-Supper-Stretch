using Project.Scripts.Util.Singleton;
using SaintsField;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.UI.Control;

[DisallowMultipleComponent]
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [field: SerializeField] public string TooltipText { private get; set; } = string.Empty;
    [field: SerializeField] public string TooltipHeader { private get; set; } = string.Empty;
    [field: SerializeField, MinValue(0)] private float Delay { get; set; } = 0.5f;
    private LTDescr? LeanTweenDelay { get; set; }

    public void OnPointerEnter(PointerEventData eventData) {
        this.LeanTweenDelay = LeanTween.delayedCall(this.Delay, () =>
                Singleton<TooltipSystem>.Instance.Show(this.TooltipText, this.TooltipHeader));
    }
    
    public void OnPointerExit(PointerEventData eventData) {
        if (this.LeanTweenDelay is not null) {
            LeanTween.cancel(this.LeanTweenDelay.uniqueId);
        }
        
        Singleton<TooltipSystem>.Instance.Hide();   
    }
}
