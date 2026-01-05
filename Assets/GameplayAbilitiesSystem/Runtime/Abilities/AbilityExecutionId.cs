using System;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    public readonly record struct AbilityExecutionId(Guid Value) {
        internal static readonly AbilityExecutionId Invalid = new AbilityExecutionId(Guid.Empty);
        
        internal static AbilityExecutionId New() => new AbilityExecutionId(Guid.NewGuid());

        public override string ToString() {
            return this.Value.ToString();
        }
        
        public static implicit operator AbilityExecutionId(Guid guid) => new AbilityExecutionId(guid);
    }
}
