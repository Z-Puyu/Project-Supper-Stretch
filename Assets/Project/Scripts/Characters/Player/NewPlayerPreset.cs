using Project.Scripts.AttributeSystem.GameplayEffects;
using SaintsField;
using Project.Scripts.Items;
using UnityEngine;

namespace Project.Scripts.Characters.Player;

[CreateAssetMenu(fileName = "New Player Preset", menuName = "Game Settings/New Player Preset")]
public class NewPlayerPreset : CharacterData {
    [field: SerializeField, SaintsDictionary("Item", "Count")]
    public SaintsDictionary<ItemData, int> StartingInventory { get; private set; } = [];
}
