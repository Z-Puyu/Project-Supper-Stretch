using UnityEngine;

namespace Project.Scripts.AbilitySystem.Abilities;

public abstract class Ability : ScriptableObject {
    [field: SerializeField]
    public string Name { get; private set; } = "";
    
    [field: SerializeField]
    public Sprite? Icon { get; private set; }
    
    [field: SerializeField]
    public string Description { get; private set; } = "";

    //[field: SerializeField]
    //public List<> Effects { get; private set; } = [];
}
