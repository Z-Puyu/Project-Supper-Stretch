using System;
using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Effects.Schedulers {
    internal abstract class EffectExecutionScheduler {
        internal int EffectStackSize { get; private set; }
        
        private protected List<KeyValuePair<GameplayAttributeType, Modifier>> Modifiers { get; } =
            new List<KeyValuePair<GameplayAttributeType, Modifier>>();
        
        private protected ModifierEnvironment? CurrentTarget { get; set; }
        internal float StartTime { get; private protected set; }
        internal abstract Guid ExecutionId { get; }
        internal abstract EffectExecutionState CurrentState { get; }
        
        internal abstract Awaitable Execute(ModifierEnvironment target, CancellationToken interrupt);

        internal EffectExecutionScheduler Schedule(
            IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> modifiers, int stackSize = 1
        ) {
            this.EffectStackSize = stackSize;
            this.Modifiers.AddRange(modifiers);
            this.StartTime = Time.time;
            return this;
        }

        private protected virtual void Reset() {
            this.EffectStackSize = 1;
            this.Modifiers.Clear();
            this.StartTime = -1;
        }
    }
}
