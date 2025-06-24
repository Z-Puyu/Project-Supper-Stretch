using UnityEngine;

namespace Project.Scripts.Characters.CharacterControl;

public class CharacterJump : MonoBehaviour {
    private bool IsJumping { get; set; }

    [field: SerializeField]
    private float JumpHeight;
}