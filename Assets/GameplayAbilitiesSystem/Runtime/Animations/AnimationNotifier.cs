using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    [CreateAssetMenu(fileName = "New Animation Notifier", menuName = "Gameplay Abilities/Animation Notifier")]
    public sealed class AnimationNotifier : ScriptableObject {
        [field: SerializeField] internal AnimationNotifier? InheritedNotifier { get; private set; }
    }
}
