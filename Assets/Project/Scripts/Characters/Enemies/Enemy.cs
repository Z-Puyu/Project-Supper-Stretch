using Project.Scripts.Characters.CharacterControl;
using UnityEngine;

namespace Project.Scripts.Characters.Enemies;

[CreateAssetMenu(fileName = "Enemy", menuName = "Character Data/Enemy")]
public class Enemy : CharacterData {
    [field: SerializeField] 
    public string Name { get; private set; } = string.Empty;
}
