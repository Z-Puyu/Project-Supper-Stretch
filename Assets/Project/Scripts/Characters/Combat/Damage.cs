using Project.Scripts.AttributeSystem.GameplayEffects;
using UnityEngine;

namespace Project.Scripts.Characters.Combat;

public readonly record struct Damage(
    Vector3 Origin,
    Vector3 HitPoint,
    float KnockBackStrength,
    int Multiplier = 100
) {
    public bool Exists => this.Multiplier > 0;

    public static Damage operator *(Damage damage, float multiplier) => damage with {
        Multiplier = damage.Multiplier == 0 ? 0 : Mathf.CeilToInt(damage.Multiplier + multiplier * 100 - 100)
    };
}
