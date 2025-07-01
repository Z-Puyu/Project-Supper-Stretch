using Project.Scripts.Common.GameplayTags;
using UnityEngine;

namespace Project.Scripts.Items.CraftingSystem;

[CreateAssetMenu(fileName = "Scheme", menuName = "Crafting/Scheme Definition", order = 0)]
public class SchemeDefinition : GameplayTagTree<Scheme>;
