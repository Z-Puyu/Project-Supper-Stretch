using System.Collections.Generic;
using System.Linq;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    internal sealed class EffectRegistry {
        private Dictionary<EffectDescriptor, List<ContinuousEffect>> ActiveEffects { get; } =
            new Dictionary<EffectDescriptor, List<ContinuousEffect>>();

        public void RegisterEffect(ContinuousEffect effect) {
            
        }
        
        public void StopEffects(Ability? ability = null, Effect? type = null, Keyword keyword = default) {
            EffectDescriptor descriptor = new EffectDescriptor(ability, type, keyword);
            List<EffectDescriptor> toRemove = this.ActiveEffects.Keys
                                                  .Where(key => key.IsOnePossibleCaseOf(descriptor))
                                                  .ToList();
            foreach (EffectDescriptor key in toRemove) {
                this.ActiveEffects.Remove(key, out List<ContinuousEffect> effects);
                effects.ForEach(effect => effect.Stop());
            }
        }
    }
}
