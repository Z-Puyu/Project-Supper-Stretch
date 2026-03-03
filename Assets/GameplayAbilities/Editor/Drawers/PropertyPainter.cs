using GameplayAbilities.Runtime.EditorTooling;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.Drawers {
    public abstract class PropertyPainter<A> : IPropertyDrawingLogic where A : CustomPropertyAttribute {
        public abstract void Paint(in VisualElement drawer, in SerializedProperty property, in A attribute);
        
        public void Apply(in VisualElement drawer, in SerialisedData data) {
            this.Paint(drawer, data.SerialisedProperty, data.GetAttribute<A>());
        }
    }
}
