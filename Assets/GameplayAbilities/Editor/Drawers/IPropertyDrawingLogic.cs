using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.Drawers {
    internal interface IPropertyDrawingLogic {
        internal void Apply(in VisualElement drawer, in SerialisedData data);
    }
}
