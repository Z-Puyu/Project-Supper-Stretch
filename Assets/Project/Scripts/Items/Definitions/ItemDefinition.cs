using Project.Scripts.Common.GameplayTags;
using UnityEngine;

namespace Project.Scripts.Items.Definitions;

[CreateAssetMenu(fileName = "Item Definition", menuName = "Item/Definition", order = 0)]
public class ItemDefinition : GameplayTagTree<ItemType>;
