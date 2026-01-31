using UnityEngine.UIElements;

namespace CommonFrameworks.Editor.CustomControls {
    internal class CommonControl : VisualElement {
        private protected CommonControl() {
            this.RegisterCallback<AttachToPanelEvent>(this.OnAttached);
            this.RegisterCallback<DetachFromPanelEvent>(this.OnDetached);
        }
        
        private protected virtual void OnAttached(AttachToPanelEvent e) { }
        private protected virtual void OnDetached(DetachFromPanelEvent e) { }
    }
}
