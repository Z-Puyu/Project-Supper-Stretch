using System.Collections.Generic;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public interface IEffectEmitterFacade {
        public IAttributeReader InstigatorAttributeSet { get; }
        public ICollection<Keyword> TagsOnEmitter { get; }
    }
}
