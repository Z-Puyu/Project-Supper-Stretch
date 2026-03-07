using System;
using System.Threading;
using GameplayAbilities.Effects.Schedulers;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Effects {
    internal readonly record struct RuntimeEffect(
        Guid Id,
        IEffect Source,
        CancellationTokenSource Interrupter,
        IScheduler Executor,
        Awaitable Task
    ) {
        internal static RuntimeEffect With(
            IEffect source, IScheduler scheduler, CancellationTokenSource interrupter, ModifierEnvironment target
        ) {
            return new RuntimeEffect(
                Guid.NewGuid(), source, interrupter, scheduler, scheduler.Execute(target, interrupter.Token)
            );
        }
    }
}
