using GameplayAbilities.Editor.UI;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.Drawers {
    public sealed class SubtypeSelectorPropertyConstructor : PropertyConstructor<SubtypeSelectorAttribute> {
        public override void Construct(
            in VisualElement drawer, in SerialisedData data, in SubtypeSelectorAttribute attribute
        ) {
            drawer.Clear();
            drawer.Add(new SubtypeSelectorPropertyField(data, attribute));
        }
    }
}
