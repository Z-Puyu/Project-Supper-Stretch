using Project.Scripts.Common.GameplayTags;
using UnityEngine;

namespace Project.Scripts.Items.Definitions;

[CreateAssetMenu(fileName = "Item Definition", menuName = "Item/Definition", order = 0)]
public class ItemDefinition : GameplayTagTree<ItemType> {
    protected override void Awake() {
        base.Awake();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
    }

    protected override void OnDisable() {
        base.OnDisable();
    }

    protected override void OnEnable() {
        base.OnEnable();
    }
}
