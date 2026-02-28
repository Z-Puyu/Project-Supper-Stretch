using UnityEngine.UIElements;

namespace CommonFrameworks.Editor.CustomControls {
    internal sealed class DropdownMenuOption : CommonControl {
        private Toggle Checkbox { get; } = new Toggle();
        private Label Label { get; } = new Label("New Item");
        private Foldout Foldout { get; } = new Foldout { toggleOnLabelClick = false };
        private VisualElement Caret { get; }
        
        private bool IsSelectable { get; }
        
        private DropdownMenuOption(bool selectable) {
            this.IsSelectable = selectable;
            this.Caret = this.Foldout.Q(className: Foldout.checkmarkUssClassName);
        }

        internal static DropdownMenuOption Create(bool selectable) {
            DropdownMenuOption option = new DropdownMenuOption(selectable) { style = { marginLeft = 4 } };
            
            option.Add(option.Foldout);
            option.Caret.pickingMode = PickingMode.Position;
            VisualElement header = option.Caret.parent;
            header.Add(option.Label);
            header.Add(option.Checkbox);
            return option;
        }
    }
}
