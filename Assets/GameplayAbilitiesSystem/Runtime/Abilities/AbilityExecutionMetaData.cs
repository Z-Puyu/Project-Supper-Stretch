using System;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    public readonly record struct AbilityExecutionMetaData(
        AbilityExecutionId Id,
        AbilitySystem Instigator,
        Ability SourceAbility,
        Action<AbilityExecutionId, AbilitySystem> OnFinished
    ) {
        public static readonly AbilityExecutionMetaData
                Invalid = new AbilityExecutionMetaData(AbilityExecutionId.Invalid, null!, null!, delegate { });

        public AbilityExecutionMetaData(
            AbilitySystem instigator, Ability ability, Action<AbilityExecutionId, AbilitySystem> onFinished
        ) : this(AbilityExecutionId.New(), instigator, ability, onFinished) { }
    }
}
