using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.Drawers {
    public sealed class CustomisablePropertyField : VisualElement {
        public VisualElement Top { get; }
        public VisualElement PropertyField { get; private set; }
        public VisualElement Bottom { get; }
        
        public CustomisablePropertyField(SerializedProperty property) {
            this.style.flexDirection = FlexDirection.Column;
            this.Top = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            this.Bottom = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            this.PropertyField = new PropertyField(property);
            this.Add(this.Top);
            this.Add(this.PropertyField);
            this.Add(this.Bottom);
        }

        public void ReplacePropertyField(VisualElement element) {
            this.Remove(this.PropertyField);
            this.PropertyField = element;
            this.Insert(1, this.PropertyField);
        }
    }
}
