using System;
using CommonFrameworks.CommonUtilities.CommonInterfaces;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects.Conditions {
    [Serializable]
    public abstract class Condition : IPredicate<(EffectSource source, EffectTarget target)> {
        [field: SerializeField]
        private ConditionSubject ExaminedEntity { get; set; } = ConditionSubject.Target;
        
        protected abstract bool HoldsForSource(EffectSource source);
        protected abstract bool HoldsForTarget(EffectTarget target);

        public bool Holds((EffectSource source, EffectTarget target) args) {
            return this.ExaminedEntity switch {
                ConditionSubject.Source => this.HoldsForSource(args.source),
                ConditionSubject.Target => this.HoldsForTarget(args.target),
                ConditionSubject.Both => this.HoldsForSource(args.source) && this.HoldsForTarget(args.target),
                var _ => false
            };
        }
    }
}
