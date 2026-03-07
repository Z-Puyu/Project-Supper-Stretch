using System;
using System.Linq;

namespace GameplayAbilities.Effects.Stacking {
    /// <summary>
    /// A stacker to replace the latest effect applied before the new one with the new effect,
    /// composing all existing modifiers into the new effect.
    /// </summary>
    [Serializable]
    internal sealed class OverrideStacking : IEffectStacker {
        public EffectStackingResult Stack(EffectExecutionState last, EffectExecutionScheme @new) {
            return new EffectStackingResult {
                ObsoleteEffect = last.Id,
                NewEffectExecutionScheme = @new with { Modifiers = last.Modifiers.Concat(@new.Modifiers) }
            };
        }
    }
}
