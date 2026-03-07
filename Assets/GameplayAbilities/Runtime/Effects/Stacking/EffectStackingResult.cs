using System;
using System.Collections.Generic;
using System.Linq;

namespace GameplayAbilities.Effects.Stacking {
    public ref struct EffectStackingResult {
        internal Guid ObsoleteEffect { get; init; }
        internal EffectExecutionScheme NewEffectExecutionScheme { get; init; }

        internal static EffectStackingResult DirectStackingOf(EffectExecutionScheme effect) {
            return new EffectStackingResult { ObsoleteEffect = Guid.Empty, NewEffectExecutionScheme = effect };
        }
    }
}
