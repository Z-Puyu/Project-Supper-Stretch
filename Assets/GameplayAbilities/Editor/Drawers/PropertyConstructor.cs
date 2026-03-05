using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.Drawers {
    public abstract class PropertyConstructor<A> : IPropertyDrawingLogic where A : CustomPropertyAttribute {
        protected abstract void Construct(in CustomisablePropertyField drawer, in SerialisedData data, in A attribute);
        
        void IPropertyDrawingLogic.Apply(in CustomisablePropertyField drawer, in SerialisedData data) {
            this.Construct(drawer, data, data.GetAttribute<A>()!);
        }
    }
}
