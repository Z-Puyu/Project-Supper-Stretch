using System.Threading;
using GameplayAbilities.Common;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Effects {
    internal interface IEffect {
        internal Awaitable Execute(
            EffectExecutionContext context, ModifierEnvironment target, 
            IUserData? userData, CancellationToken interrupt
        );
    }
}
