using System;
using GameplayAbilitiesSystem.Runtime.Abilities;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    [Serializable]
    public abstract class AnimationEventHandler {
        [field: SerializeField, Required] internal AnimationNotifier? HandledNotifier { get; private set; }
        
        public abstract void Handle(AbilitySystem system, AnimationNotifier notifier);
    }
}
