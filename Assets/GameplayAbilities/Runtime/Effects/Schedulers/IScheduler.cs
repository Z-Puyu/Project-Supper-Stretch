using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Abilities;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Effects.Schedulers {
    internal interface IScheduler {
        internal Awaitable Execute(ModifierEnvironment target, CancellationToken interrupt);
        
        internal IScheduler Clone(IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> modifiers);
    }
}
