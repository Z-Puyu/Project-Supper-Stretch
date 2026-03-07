namespace GameplayAbilities.Effects.Stacking {
    public interface IEffectStacker {
        /// <summary>
        /// Stacks the new effect on top of the last one.
        /// </summary>
        /// <param name="last">The current execution state of the latest effect applied before the new one.</param>
        /// <param name="new">The new effect to be stacked on top of the last one.</param>
        /// <returns>A <see cref="EffectStackingResult"/> indicating the result of the stacking operation.</returns>
        internal EffectStackingResult Stack(EffectExecutionState last, EffectExecutionScheme @new);
    }
}
