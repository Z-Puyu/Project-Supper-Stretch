using System.Collections.Generic;

namespace Project.Scripts.Characters.CharacterControl.Combat;

public interface IDamageDealer {
    public abstract void Damage(IEnumerable<IDamageable> targets);
    
    public abstract void Damage(IDamageable target);
}
