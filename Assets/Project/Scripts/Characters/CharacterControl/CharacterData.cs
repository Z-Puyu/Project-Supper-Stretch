using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using UnityEngine;

namespace Project.Scripts.Characters.CharacterControl;

public class CharacterData : ScriptableObject {
    [field: SerializeField]
    public List<CharacterAttributeData> Attributes { get; private set; } = [];
}
