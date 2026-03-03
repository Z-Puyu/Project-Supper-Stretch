using UnityEditor;
using UnityEngine.UIElements;

namespace GameplayAbilities.Runtime.EditorTooling {
    public interface IPropertyModifier {
        public void Decorate(VisualElement drawer, SerializedProperty property);
    }
}
