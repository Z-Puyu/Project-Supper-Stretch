using GameplayAbilities.Runtime.EditorTooling;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.Drawers {
    public abstract class PropertyPainter<A> : IPropertyDrawingLogic where A : CustomPropertyAttribute {
        protected abstract void Paint(CustomisablePropertyField drawer, SerialisedData data, A attribute);
        
        public void Apply(in CustomisablePropertyField drawer, in SerialisedData data) {
            this.Paint(drawer, data, data.GetAttribute<A>()!);
        }
    }
}
