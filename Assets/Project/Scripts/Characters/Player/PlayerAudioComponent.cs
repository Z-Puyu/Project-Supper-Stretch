using Project.Scripts.Audio;
using UnityEngine;

namespace Project.Scripts.Characters.Player;

public class PlayerAudioComponent : AudioPlayer<PlayerAudioComponent.Sound> {
    public enum Sound {
        Attack, Hurt
    }
}
