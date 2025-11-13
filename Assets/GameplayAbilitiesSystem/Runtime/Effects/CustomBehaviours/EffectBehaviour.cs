using System;
using System.Collections.Generic;
using CommonFrameworks.CommonUtilities.CommonInterfaces;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects.CustomBehaviours {
    [Serializable]
    public abstract class EffectBehaviour : IEffect<EffectTarget> {
        [field: SerializeReference, ReferencePicker]
        private List<IPredicate<(EffectSource source, EffectTarget target)>> Conditions { get; set; } =
            new List<IPredicate<(EffectSource source, EffectTarget target)>>();

        public virtual bool IsApplicable(EffectSource source, EffectTarget target) {
            return this.Conditions.Count == 0 ||
                   this.Conditions.TrueForAll(condition => condition.Holds((source, target)));
        }
        
        public abstract void Apply(EffectTarget target);
        public abstract void Stop();
    }
}
