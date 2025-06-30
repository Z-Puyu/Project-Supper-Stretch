using System;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Audio;

public class AmbientTrack : MonoBehaviour {
    [NotNull] 
    [field: SerializeField, Required] 
    private AudioSource? BackgroundMusic { get; set; }
    
    [NotNull] 
    [field: SerializeField, Required] 
    private AudioSource? CombatMusic { get; set; }

    private void Start() {
        this.CombatMusic.volume = 0;
        this.BackgroundMusic.volume = 0;
        this.CombatMusic.Play();
        this.BackgroundMusic.Play();
        LeanTween.value(this.BackgroundMusic.gameObject, value => this.BackgroundMusic.volume = value, 0, 1, 2f);
    }

    public void TransitionToCombat() {
        LeanTween.value(this.BackgroundMusic.gameObject, value => this.BackgroundMusic.volume = value,
            this.BackgroundMusic.volume, 0, 2f);
        LeanTween.value(this.BackgroundMusic.gameObject, value => this.CombatMusic.volume = value, 0, 1, 2f);
    }
    
    public void TransitionToBackground() {
        LeanTween.value(this.BackgroundMusic.gameObject, value => this.BackgroundMusic.volume = value, 0, 1, 2f);
        LeanTween.value(this.BackgroundMusic.gameObject, value => this.CombatMusic.volume = value,
            this.CombatMusic.volume, 0, 2f);
    }
}
