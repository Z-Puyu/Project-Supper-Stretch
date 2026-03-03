using System.Threading;
using GameplayAbilities.Abilities;
using GameplayAbilities.Common;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Effects {
    public interface IEffect {
        internal Awaitable Execute(
            EffectExecutionContext context, ModifierEnvironment target, 
            IUserData? userData, CancellationToken interrupt
        );
    }
}
