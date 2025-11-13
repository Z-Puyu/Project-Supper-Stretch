using System.Collections.Generic;
using CommonFrameworks.CommonUtilities.CommonInterfaces;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public abstract class Effect : ScriptableObject, IEffect<EffectSource, EffectTarget> {
        public abstract void Apply(EffectSource source, EffectTarget target);
    }
}
