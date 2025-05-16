using UnityEngine;

namespace Project.Scripts.Player;

public class CharacterAttack : MonoBehaviour {
    [field: SerializeField] 
    private Animator? Animator { get; set; }
    
    public void Attack(int animId) {
        this.Animator?.SetTrigger(animId);
    }
}