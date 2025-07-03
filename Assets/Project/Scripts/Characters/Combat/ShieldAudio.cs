using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Audio;

namespace Project.Scripts.Characters.Combat;

public class ShieldAudio : AudioPlayer<ShieldAudio.Sound> {
    public enum Sound { Hold, Block, Parry }

    [NotNull] private BlockingZone? Shield { get; set; }
    
    protected override void Awake() {
        base.Awake();
        this.Shield = this.GetComponentInParent<BlockingZone>();
    }

    private void Start() {
        this.Shield.OnHeldUp += () => this.Play(Sound.Hold, 1);
        this.Shield.OnBlocked += () => this.Play(Sound.Block, 1);
        this.Shield.OnParried += () => this.Play(Sound.Parry, 1);       
    }
}
