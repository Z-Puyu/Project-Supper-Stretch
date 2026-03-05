using GameplayAbilities.Editor.UI;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine.UIElements;

namespace GameplayAbilities.Editor.Drawers {
    public sealed class InlinePropertyConstructor : PropertyConstructor<InlineAttribute> {
        protected override void Construct(
            in CustomisablePropertyField drawer, in SerialisedData data, in InlineAttribute attribute
        ) {
            drawer.Clear();
            drawer.Add(new InlinePropertyField(data.SerialisedProperty));
        }
    }
}
