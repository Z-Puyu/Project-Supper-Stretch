using System;
using CommonFrameworks.CommonUtilities.Logic;

namespace GameplayAbilitiesSystem.Runtime.Effects.Conditions {
    [Serializable]
    internal sealed class And : AndCondition<(EffectSource source, EffectTarget target)> { }
}
