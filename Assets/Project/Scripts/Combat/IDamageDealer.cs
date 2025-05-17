using UnityEngine;

namespace Project.Scripts.Combat;

public interface IDamageDealer {
    public abstract void Damage(IDamageable target);
}
