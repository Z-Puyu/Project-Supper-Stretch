using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using UnityEngine;

namespace Project.Scripts.Characters;

public class CharacterData : ScriptableObject {
    [field: SerializeField]
    public List<AttributeInitialisationData> Attributes { get; private set; } = [];
}
