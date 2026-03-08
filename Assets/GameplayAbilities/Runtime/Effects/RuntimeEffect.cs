using System;
using System.Threading;
using GameplayAbilities.Effects.Schedulers;
using UnityEngine;

namespace GameplayAbilities.Effects {
    internal readonly record struct RuntimeEffect(
        Guid Id,
        IEffect Source,
        CancellationTokenSource Interrupter,
        EffectExecutionScheduler Executor,
        Awaitable Task
    );
}
