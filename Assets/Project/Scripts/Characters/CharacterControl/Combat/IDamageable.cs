using UnityEngine;

namespace Project.Scripts.Combat;

public interface IDamageable {
    public bool CanBeDamagedBy<T>(T damager) where T : Component, IDamageDealer;
    
    public void TakeDamage(int damage, GameObject? source = null);
    
    public void Heal(int amount, GameObject? source);
    
    public void Die();
}
