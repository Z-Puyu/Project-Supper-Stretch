using System;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Modifiers;

public class DoubleAttributeBasedMagnitude<S, T> : Magnitude where S : Enum where T : Enum {
    private enum Combiner {
        Additive,
        Multiplicative,
        Max,
        Min,
        Average,
        Difference,
        SelfToTargetRatio,
        TargetToSelfRatio,
        SelfMinusTarget,
        TargetMinusSelf
    }
    
    private Attributes.AttributeManagementSystem? Self { get; set; }
    private Attributes.AttributeManagementSystem? Target { get; set; }
    
    [field: SerializeField]
    private S SelfAttribute { get; set; }

    [field: SerializeField]
    private float SelfCoefficient { get; set; } = 1;
    
    [field: SerializeField]
    private T TargetAttribute { get; set; }
    
    [field: SerializeField]
    private float TargetCoefficient { get; set; } = 1;
    
    [field: SerializeField]
    private Combiner CombinerType { get; set; }
    
    [field: SerializeField] 
    private float OverallCoefficient { get; set; }

    private DoubleAttributeBasedMagnitude(S selfAttribute, T targetAttribute) {
        this.SelfAttribute = selfAttribute;
        this.TargetAttribute = targetAttribute;
    }

    private float Calculate(float selfValue, float targetValue) {
        return this.CombinerType switch {
            Combiner.Additive => selfValue + targetValue,
            Combiner.Multiplicative => selfValue * targetValue,
            Combiner.Max => Mathf.Max(selfValue, targetValue),
            Combiner.Min => Mathf.Min(selfValue, targetValue),
            Combiner.Average => (selfValue + targetValue) / 2,
            Combiner.Difference => Mathf.Abs(selfValue - targetValue),
            Combiner.SelfToTargetRatio => targetValue == 0 ? float.MaxValue : selfValue / targetValue,
            Combiner.TargetToSelfRatio => selfValue == 0 ? float.MaxValue : targetValue / selfValue,
            Combiner.SelfMinusTarget => selfValue - targetValue,
            Combiner.TargetMinusSelf => targetValue - selfValue,
            var _ => 0
        };
    }

    public override float Evaluate() {
        if (this.Self == null || this.Target == null) {
            return 0;
        }
        
        float selfVal = this.Self.Query(this.SelfAttribute).CurrentValue * this.SelfCoefficient;
        float targetVal = this.Target.Query(this.TargetAttribute).CurrentValue * this.TargetCoefficient;
        return this.OverallCoefficient * this.Calculate(selfVal, targetVal);
    }

    public override float Evaluate(Enum tag) {
        if (this.Self == null || this.Target == null) {
            return 0;
        }
        
        float selfVal = this.Self.Query(tag, this.SelfAttribute).CurrentValue * this.SelfCoefficient;
        float targetVal = this.Target.Query(tag, this.TargetAttribute).CurrentValue * this.TargetCoefficient;
        return this.OverallCoefficient * this.Calculate(selfVal, targetVal);
    }

    public override Magnitude BasedOn(Attributes.AttributeManagementSystem? self, Attributes.AttributeManagementSystem target) {
        return self == null
                ? this
                : new DoubleAttributeBasedMagnitude<S, T>(this.SelfAttribute, this.TargetAttribute) {
                    Self = self,
                    Target = target,
                    SelfCoefficient = this.SelfCoefficient,
                    TargetCoefficient = this.TargetCoefficient,
                    CombinerType = this.CombinerType,
                    OverallCoefficient = this.OverallCoefficient
                };
    }
}
