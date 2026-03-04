using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.Drawers {
    public abstract class PropertyConstructor<A> : IPropertyDrawingLogic where A : CustomPropertyAttribute {
        public abstract void Construct(in VisualElement drawer, in SerialisedData data, in A attribute);
        
        void IPropertyDrawingLogic.Apply(in VisualElement drawer, in SerialisedData data) {
            this.Construct(drawer, data, data.GetAttribute<A>());
        }
    }
}
