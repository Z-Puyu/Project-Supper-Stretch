using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.Drawers {
    internal interface IPropertyDrawingLogic {
        internal void Apply(in CustomisablePropertyField drawer, in SerialisedData data);
    }
}
