using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;

namespace GameplayAbilities.Effects {
    public readonly record struct EffectExecutionContext(
        CapturedAttributes SourceAttributes,
        CapturedAttributes TargetAttributes,
        ModifierEnvironment Target
    ) {
        public EffectExecutionContext(IAttributeReader source, IAttributeReader target, ModifierEnvironment environment)
                : this(CapturedAttributes.From(source), CapturedAttributes.From(target), environment) { }
    }
}
