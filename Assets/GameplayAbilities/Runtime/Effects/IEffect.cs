using System.Collections.Generic;
using System.Threading;
using GameplayAbilities.Abilities;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Effects {
    public interface IEffect {
        internal Awaitable Execute(
            EffectExecutionContext context, ModifierEnvironment target, 
            AbilityExecutionUserData? userData, CancellationToken interrupt
        );
    }
}
