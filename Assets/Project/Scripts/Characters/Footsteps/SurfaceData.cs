using UnityEngine;
using UnityEngine.Audio;

namespace Project.Scripts.Characters.Footsteps;

[CreateAssetMenu(fileName = "Surface Data", menuName = "Footsteps/Surface Data", order = 0)]
public class SurfaceData : ScriptableObject {
    [field: SerializeField] private AudioResource? DefaultSound { get; set; }
    [field: SerializeField] private AudioResource? RunningSound { get; set; }
    [field: SerializeField] private AudioResource? WalkingSound { get; set; }

    public AudioResource PullWalkingSound() {
        return this.WalkingSound ? this.WalkingSound : this.DefaultSound!;
    }

    public AudioResource PullRunningSound() {
        return this.RunningSound ? this.RunningSound : this.DefaultSound!;       
    }
}
