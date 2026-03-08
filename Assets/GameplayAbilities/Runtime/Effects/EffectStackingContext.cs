using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;

namespace GameplayAbilities.Effects {
    internal readonly ref struct EffectStackingContext {
        internal EffectExecutionState CurrentExecutionState { get; init; }
        internal EffectExecutionContext NewEffectExecutionContext { get; init; }
        internal CancellationTokenSource NewEffectInterrupter { get; init; }
        
        internal int CurrentStackSize => this.CurrentExecutionState.StackSize;
        internal ModifierEnvironment CurrentTarget => this.CurrentExecutionState.Target;
        internal int RemainingTicks => this.CurrentExecutionState.RemainingTicks;
        internal float RemainingDuration => this.CurrentExecutionState.RemainingDuration;
        internal float WaitingTimeUntilNextTick => this.CurrentExecutionState.WaitingTimeUntilNextTick;
        
        internal IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> CurrentModifiers =>
                this.CurrentExecutionState.Modifiers;
    }
}
