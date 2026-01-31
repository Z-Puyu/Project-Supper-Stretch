using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CommonFrameworks.Editor.CustomControls {
    [UxmlElement]
    internal partial class DropdownMenuItem : CommonControl {
        private Foldout Foldout { get; } = new Foldout { toggleOnLabelClick = false };
        private VisualElement Caret { get; }
        private Toggle Checkbox { get; } = new Toggle();
        private Label Label { get; } = new Label("New Item");
        
        private object? Item { get; set; }
        
        public DropdownMenuItem() {
            this.Add(this.Foldout);
            this.Caret = this.Q(className: Foldout.checkmarkUssClassName);
            this.Caret.pickingMode = PickingMode.Position;
            VisualElement header = this.Caret.parent;
            header.Add(this.Label);
            header.Add(this.Checkbox);
            this.RegisterCallback<PointerDownEvent>(this.OnPointerDown, TrickleDown.TrickleDown);
        }

        internal DropdownMenuItem(string label, object item) : this() {
            this.Label.text = label;
            this.Item = item;
        }
        
        private void OnPointerDown(PointerDownEvent e) {
            if (e.currentTarget != this) {
                return;
            }
            
            this.Label.text = e.target.ToString();
            if (e.target == this.Caret || e.target == this.Checkbox) {
                return;
            }

            this.Checkbox.value = !this.Checkbox.value;
            e.StopPropagation();
        }
    }
}
