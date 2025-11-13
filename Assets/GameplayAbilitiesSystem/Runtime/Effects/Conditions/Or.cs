using System;
using CommonFrameworks.CommonUtilities.CommonInterfaces;
using CommonFrameworks.CommonUtilities.Logic;

namespace GameplayAbilitiesSystem.Runtime.Effects.Conditions {
    [Serializable]
    internal sealed class Or : OrCondition<(EffectSource source, EffectTarget target)> { }
}
