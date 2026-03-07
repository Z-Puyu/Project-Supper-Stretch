using GameplayAbilities.Attributes;

namespace GameplayAbilities.Effects {
    public readonly record struct EffectExecutionContext(
        CapturedAttributes SourceAttributes,
        CapturedAttributes TargetAttributes
    ) {
        public EffectExecutionContext(IAttributeReader source, IAttributeReader target)
                : this(CapturedAttributes.From(source), CapturedAttributes.From(target)) { }

        public static EffectExecutionContext FromSelfOnSelf(IAttributeReader attributes) {
            return new EffectExecutionContext(attributes, attributes);
        }
    }
}
