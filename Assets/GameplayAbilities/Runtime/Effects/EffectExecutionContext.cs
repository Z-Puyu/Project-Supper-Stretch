using GameplayAbilities.Attributes;
using GameplayAbilities.Common;

namespace GameplayAbilities.Effects {
    public readonly record struct EffectExecutionContext(
        CapturedAttributes SourceAttributes,
        CapturedAttributes TargetAttributes,
        IUserData? UserData = null
    ) {
        public EffectExecutionContext(IAttributeReader source, IAttributeReader target, IUserData? userData = null)
                : this(CapturedAttributes.From(source), CapturedAttributes.From(target), userData) { }

        public static EffectExecutionContext FromSelfOnSelf(IAttributeReader attributes, IUserData? userData = null) {
            return new EffectExecutionContext(attributes, attributes, userData);
        }
    }
}
