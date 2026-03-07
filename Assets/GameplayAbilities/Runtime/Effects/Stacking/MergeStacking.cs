using System;
using System.Linq;

namespace GameplayAbilities.Effects.Stacking {
    /// <summary>
    /// A stacker that merges the new effect with the latest effect applied before it.
    /// The modifiers from both effects are composed, and the duration of the existing effect is extended.
    /// </summary>
    [Serializable]
    internal sealed class MergeStacking : IEffectStacker {
        public EffectStackingResult Stack(EffectExecutionState last, EffectExecutionScheme @new) {
            return new EffectStackingResult {
                ObsoleteEffect = last.Id,
                NewEffectExecutionScheme = new EffectExecutionScheme {
                    Modifiers = @new.Modifiers.Concat(last.Modifiers),
                    ExecutionSchedule = last + @new.ExecutionSchedule,
                }
            };
        }
    }
}
