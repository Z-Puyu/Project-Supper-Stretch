using System.Collections.Generic;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Characters.Enemies;

[CreateAssetMenu(fileName = "Enemy", menuName = "Character Data/Enemy")]
public class Enemy : ScriptableObject {
    [field: SerializeField] 
    public string Name { get; private set; } = string.Empty;
    
    [field: SerializeField] 
    public List<CharacterAttributeData> Attributes { get; private set; } = [];
}
