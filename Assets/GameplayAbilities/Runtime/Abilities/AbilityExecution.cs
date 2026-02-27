using System;
using System.Collections.Generic;

namespace GameplayAbilities.Abilities {
    [Serializable]
    public abstract class AbilityExecution {
        protected internal abstract void Execute(AbilitySystem source, IReadOnlyDictionary<string, double>? userData);
    }
}
