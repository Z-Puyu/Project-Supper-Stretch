using System;
using GameplayAbilities.Attributes;
using GameplayAbilities.Attributes.Evaluation;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [Serializable]
    internal sealed class Cost {
        private enum Verdict {
            [InspectorName("Has Enough")] HasEnough,
            [InspectorName("More than Enough")] MoreThanEnough,
            [InspectorName("Has Any")] HasAny,
            [InspectorName("Has Room for More")] HasRoomForMore,
            [InspectorName("Has Enough Room")] HasEnoughRoom,
            
            [InspectorName("More Room than Necessary")] 
            MoreRoomThanNecessary
        }
        
        [field: SerializeField] private Verdict BenchmarkScheme { get; set; } = Verdict.HasEnough;
        [field: SerializeField] private GameplayAttributeType? CostAttribute { get; set; }
        [field: SerializeReference] private IAttributeMagnitude? Amount { get; set; } = new Constant();
        
        
        internal bool IsAffordable(IAttributeReader consumer) {
            if (!this.CostAttribute) {
                return true;
            }
            
            double cost = this.Amount?.Evaluate(consumer) ?? 0;
            const double d = 0.001;
            return this.BenchmarkScheme switch {
                Verdict.HasEnough => consumer.HasAtLeast(cost, this.CostAttribute),
                Verdict.MoreThanEnough => consumer.HasAtLeast(cost + d, this.CostAttribute),
                Verdict.HasAny => consumer.HasAtLeast(consumer.QueryMin(this.CostAttribute) + d, this.CostAttribute),
                Verdict.HasRoomForMore => consumer.HasAtMost(
                    consumer.QueryMax(this.CostAttribute) - d, this.CostAttribute
                ),
                Verdict.HasEnoughRoom => consumer.HasAtMost(
                    consumer.QueryMax(this.CostAttribute) - cost, this.CostAttribute
                ),
                Verdict.MoreRoomThanNecessary => consumer.HasAtMost(
                    consumer.QueryMax(this.CostAttribute) - cost - d, this.CostAttribute
                ),
                var _ => false
            };
        }
        
        internal void Spend(IAttributeReader consumer, IModifiable wallet) {
            if (!this.CostAttribute) {
                return;
            }
            
            double cost = this.Amount?.Evaluate(consumer) ?? 0;
            double change = this.BenchmarkScheme switch {
                Verdict.HasEnough or Verdict.MoreThanEnough or Verdict.HasAny => -cost,
                Verdict.HasRoomForMore or Verdict.HasEnoughRoom or Verdict.MoreRoomThanNecessary => cost,
                var _ => 0
            };
            
            wallet.AddModifier(this.CostAttribute, new Modifier(ModifierType.Offset, change));
        }
    }
}
