using UnityEngine;

namespace Project.Scripts.Player;

public class CharacterJump : MonoBehaviour {
    private bool IsJumping { get; set; }

    [field: SerializeField]
    private float JumpHeight;
}