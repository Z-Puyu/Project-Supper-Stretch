using System.Diagnostics.CodeAnalysis;
using Project.Scripts.Audio;

namespace Project.Scripts.Characters.Combat;

public class WeaponAudio : AudioPlayer<WeaponAudio.Sound> {
    public enum Sound { Attack, Blocked }

    [NotNull] private DamageDealer? DamageDealer { get; set; }
    
    protected override void Awake() {
        base.Awake();
        this.DamageDealer = this.GetComponentInParent<DamageDealer>();
    }

    private void Start() {
        this.DamageDealer.OnAttack += () => this.Play(Sound.Attack);
        this.DamageDealer.OnBlocked += () => this.Play(Sound.Blocked);
        this.DamageDealer.OnKnockedBack += () => this.Play(Sound.Blocked);
    }
}
