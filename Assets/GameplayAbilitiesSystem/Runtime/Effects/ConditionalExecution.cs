using System.Collections.Generic;
using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects;

public abstract class ConditionalExecution {
    [field: SerializeReference, ReferencePicker]
    private List<IPredicate<EffectSource>> SourceConditions { get; set; } = new List<IPredicate<EffectSource>>();

    [field: SerializeReference, ReferencePicker]
    protected List<IPredicate<EffectTarget>> TargetConditions { get; private set; } = new List<IPredicate<EffectTarget>>();
        
    protected virtual bool IsApplicable(EffectTarget target) {
        return this.TargetConditions.Count == 0 ||
               this.TargetConditions.TrueForAll(condition => condition.Holds(target));
    }
        
    protected virtual bool IsApplicable(EffectSource source) {
        return this.SourceConditions.Count == 0 ||
               this.SourceConditions.TrueForAll(condition => condition.Holds(source));
    }

    public bool IsApplicable(EffectSource source, EffectTarget target) {
        return this.IsApplicable(target) && this.IsApplicable(source);
    }
}