using GameplayAbilities.Editor.UI;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.Drawers {
    public sealed class SubtypeSelectorPropertyConstructor : PropertyConstructor<SubtypeSelectorAttribute> {
        protected override void Construct(
            in CustomisablePropertyField drawer, in SerialisedData data, in SubtypeSelectorAttribute attribute
        ) {
            drawer.Clear();
            drawer.Add(new SubtypeSelectorPropertyField(data, attribute));
        }
    }
}
