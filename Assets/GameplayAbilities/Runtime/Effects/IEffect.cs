using System;
using System.Threading;
using GameplayAbilities.Common;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Effects {
    internal interface IEffect {
        internal RuntimeEffect Execute(
            EffectExecutionScheme scheme, ModifierEnvironment target, CancellationTokenSource interrupter
        );
        
        internal EffectExecutionScheme CreateExecutionScheme(EffectExecutionContext context, IUserData? userData);
    }
}
