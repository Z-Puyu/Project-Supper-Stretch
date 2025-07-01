using Project.Scripts.Common.GameplayTags;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.Definitions;

[CreateAssetMenu(fileName = "AttributeDefinition", menuName = "Attribute System/Attribute Definition", order = 0)]
public class AttributeDefinition : GameplayTagTree<AttributeType> {
    private void Awake() {
        PreorderIterator<AttributeType> iterator = new PreorderIterator<AttributeType>(this.Nodes);
        iterator.ForEach = cleanup;
        this.Traverse(iterator);
        return;

        void cleanup(AttributeType tag) {
            if (tag.HowToClamp != AttributeType.ClampPolicy.CapByAttribute) {
                tag.Cap = string.Empty;
            } else if (tag.HowToClamp != AttributeType.ClampPolicy.CapByValue) {
                tag.MaxValue = -1;
            }
        }
    }
}
