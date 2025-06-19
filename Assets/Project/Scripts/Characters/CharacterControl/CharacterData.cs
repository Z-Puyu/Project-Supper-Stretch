using System.Collections.Generic;
using SaintsField;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using UnityEngine;

namespace Project.Scripts.Characters.CharacterControl;

public class CharacterData : ScriptableObject {
    [field: SerializeField, Table]
    public List<AttributeInitialisationData> Attributes { get; private set; } = [];
}
