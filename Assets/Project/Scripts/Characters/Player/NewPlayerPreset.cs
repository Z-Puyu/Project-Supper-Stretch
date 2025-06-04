using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.Items;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.Player;

[CreateAssetMenu(fileName = "New Player Preset", menuName = "Game Settings/New Player Preset")]
public class NewPlayerPreset : ScriptableObject {
    [field: SerializeField]
    public List<CharacterAttributeData> InitialStats { get; private set; } = [];
    
    [field: SerializeField, SaintsDictionary("Item", "Count")]
    public SaintsDictionary<Item, int> StartingInventory { get; private set; } = [];
}
