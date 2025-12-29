using System.Collections.Generic;
using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    public abstract class ConditionalExecution {
        [field: SerializeReference, ReferencePicker]
        private List<IPredicate<EffectEmitterFacade>> SourceConditions { get; set; } = new List<IPredicate<EffectEmitterFacade>>();

        [field: SerializeReference, ReferencePicker]
        protected List<IPredicate<EffectReceiverFacade>> TargetConditions { get; private set; } = new List<IPredicate<EffectReceiverFacade>>();
        
        protected virtual bool IsApplicable(IEffectReceiverFacade target) {
            return this.TargetConditions.Count == 0 ||
                   this.TargetConditions.TrueForAll(condition => condition.Holds(target));
        }
        
        protected virtual bool IsApplicable(IEffectEmitterFacade source) {
            return this.SourceConditions.Count == 0 ||
                   this.SourceConditions.TrueForAll(condition => condition.Holds(source));
        }

        public bool IsApplicable(EffectEmitterFacade source, EffectReceiverFacade target) {
            return this.IsApplicable(target) && this.IsApplicable(source);
        }
    }
}