using System;
using System.Threading;
using GameplayAbilities.Common;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [Serializable]
    public abstract class AbilityExecution {
        protected internal abstract Awaitable Execute(
            AbilitySystemController source, IUserData? userData, CancellationToken interrupt
        );
    }
}
