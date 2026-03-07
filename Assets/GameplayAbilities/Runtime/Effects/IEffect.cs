using System.Threading;
using GameplayAbilities.Modifiers;

namespace GameplayAbilities.Effects {
    internal interface IEffect {
        internal RuntimeEffect Execute(
            EffectExecutionScheme scheme, ModifierEnvironment target, CancellationTokenSource interrupter
        );
    }
}
