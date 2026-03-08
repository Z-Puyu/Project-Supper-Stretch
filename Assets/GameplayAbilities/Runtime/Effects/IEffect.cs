using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;

namespace GameplayAbilities.Effects {
    internal interface IEffect {
        internal RuntimeEffect Execute(
            EffectExecutionContext context, ModifierEnvironment target, CancellationTokenSource interrupter
        );
    }
}
