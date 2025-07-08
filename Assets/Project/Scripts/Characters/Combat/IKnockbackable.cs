using UnityEngine;

namespace Project.Scripts.Characters.Combat;

public interface IKnockBackable {
    public abstract void GetKnockedBack(Vector3 force);    
}
