using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Util.Singleton;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Audio;

[DisallowMultipleComponent]
public class AmbientTrack : Singleton<AmbientTrack> {
    [NotNull]
    [field: SerializeField, Required] 
    private PlayList? BackgroundMusic { get; set; }
    
    [NotNull]
    [field: SerializeField, Required] 
    private PlayList? CombatMusic { get; set; }
    
    private void Start() {
        this.BackgroundMusic.Stop();
        this.CombatMusic.Stop();
        this.BackgroundMusic.FadeInNext();
    }

    public void TransitionToCombat() {
        this.BackgroundMusic.FadeOut();
        this.CombatMusic.FadeInNext();
    }
    
    public void TransitionToBackground() {
        this.CombatMusic.FadeOut();
        this.BackgroundMusic.FadeInNext();   
    }
}
