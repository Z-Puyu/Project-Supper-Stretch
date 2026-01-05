using System;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    [Serializable]
    public record struct AnimationResource {
        [field: SerializeField] internal AnimationClip Clip { get; set; }
        [field: SerializeField] internal AnimationSignal SignalOnBeginPlay { get; set; }
        [field: SerializeField] internal AnimationSignal SignalOnEndPlay { get; set; }
    }
}
