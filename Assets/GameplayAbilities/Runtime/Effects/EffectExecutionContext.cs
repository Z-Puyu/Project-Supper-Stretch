using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;

namespace GameplayAbilities.Effects {
    public readonly record struct EffectExecutionContext(
        ReadOnlyAttributeSet SourceAttributes,
        ReadOnlyAttributeSet TargetAttributes,
        ModifierEnvironment Target
    ) {
        public EffectExecutionContext(IAttributeReader source, IAttributeReader target, ModifierEnvironment environment)
                : this(ReadOnlyAttributeSet.From(source), ReadOnlyAttributeSet.From(target), environment) { }
    }
}
