using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.Characters.CharacterControl;
using UnityEngine;

namespace Project.Scripts.Characters;

public readonly record struct Damage(GameplayEffect? Effect, GameObject? Source = null, int Multiplier = 100) {
    public bool Exists => this.Multiplier > 0;
    
    public static Damage operator *(Damage damage, float multiplier) => damage with {
        Multiplier = damage.Multiplier == 0 ? 0 : Mathf.CeilToInt(damage.Multiplier + multiplier * 100 - 100)
    };
}
