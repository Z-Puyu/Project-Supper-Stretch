using System;

namespace GameplayAbilities.Effects.Stacking {
    /// <summary>
    /// A stacker that extends the duration of the latest effect applied before the new effect
    /// by the duration of the new effect.
    /// </summary>
    [Serializable]
    internal sealed class ExtendStacking : IEffectStacker {
        public EffectStackingResult Stack(EffectExecutionState last, EffectExecutionScheme @new) {
            return new EffectStackingResult {
                ObsoleteEffect = last.Id,
                NewEffectExecutionScheme = new EffectExecutionScheme {
                    Modifiers = last.Modifiers,
                    ExecutionSchedule = (last + @new.ExecutionSchedule) with { ShouldTickOnStart = true },
                }
            };
        }
    }
}
