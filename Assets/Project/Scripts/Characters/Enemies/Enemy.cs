using Project.Scripts.Items.InventorySystem.LootContainers;
using UnityEngine;

namespace Project.Scripts.Characters.Enemies;

[CreateAssetMenu(fileName = "Enemy", menuName = "Character Data/Enemy")]
public class Enemy : CharacterData {
    [field: SerializeField] public string Name { get; private set; } = string.Empty;
    [field: SerializeField] public int Experience { get; private set; }
    [field: SerializeField] public LootTable? LootTable { get; private set; }
}
