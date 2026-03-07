using System;

namespace GameplayAbilities.Effects.Stacking {
    /// <summary>
    /// A stacker that applies the new effect on top of all existing ones independently.
    /// </summary>
    [Serializable]
    internal sealed class IndependentStacking : IEffectStacker {
        EffectStackingResult IEffectStacker.Stack(EffectExecutionState last, EffectExecutionScheme @new) {
            return EffectStackingResult.DirectStackingOf(@new);
        }
    }
}
