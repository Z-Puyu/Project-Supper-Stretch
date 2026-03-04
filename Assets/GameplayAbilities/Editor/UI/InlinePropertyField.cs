using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.UI {
    public sealed class InlinePropertyField : VisualElement {
        public InlinePropertyField(SerializedProperty property) {
            if (property.propertyType is SerializedPropertyType.Generic or SerializedPropertyType.ManagedReference) {
                SerializedProperty iterator = property.Copy();
                SerializedProperty end = iterator.GetEndProperty();
                if (!iterator.NextVisible(true) || SerializedProperty.EqualContents(iterator, end)) {
                    return;
                }

                do {
                    PropertyField field = new PropertyField(iterator);
                    field.BindProperty(iterator);
                    this.Add(field);
                } while (iterator.NextVisible(false) && !SerializedProperty.EqualContents(iterator, end));
            } else {
                PropertyField field = new PropertyField(property);
                field.BindProperty(property);
                this.Add(field);
            }
        }
    }
}
