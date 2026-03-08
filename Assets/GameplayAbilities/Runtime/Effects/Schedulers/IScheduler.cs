using System;
using System.Threading;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Effects.Schedulers {
    internal interface IScheduler {
        internal Guid ExecutionId { get; }
        internal Awaitable Execute(ModifierEnvironment target, CancellationToken interrupt);
        internal IScheduler Schedule(EffectExecutionScheme scheme);
        internal EffectExecutionSchedule ExecutionSchedule { get; }
        internal EffectExecutionState CurrentState { get; }
    }
}
