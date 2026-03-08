using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Attributes.Evaluation;
using GameplayAbilities.Common;
using GameplayAbilities.Effects;
using GameplayAbilities.Effects.Schedulers;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [Serializable]
    internal sealed class Cost : IEffect {
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
        
        internal bool IsAffordable(IAttributeReader consumer, IUserData? userData) {
            if (!this.CostAttribute) {
                return true;
            }
            
            double cost = this.Amount?.Evaluate(consumer, userData) ?? 0;
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

        RuntimeEffect IEffect.Execute(
            EffectExecutionContext context, ModifierEnvironment target, CancellationTokenSource interrupter
        ) {
            EffectExecutionScheduler executor = InstantExecution.NewInstance.Schedule(this.MakeModifiers(context));
            return new RuntimeEffect {
                Id = Guid.NewGuid(),
                Source = this,
                Executor = executor,
                Interrupter = interrupter,
                Task = executor.Execute(target, interrupter.Token)
            };
        }

        private IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> MakeModifiers(
            EffectExecutionContext context
        ) {
            if (!this.CostAttribute) {
                return Enumerable.Empty<KeyValuePair<GameplayAttributeType, Modifier>>();
            }
            
            double cost = this.Amount?.Evaluate(context.TargetAttributes, context.UserData) ?? 0;
            double change = this.BenchmarkScheme switch {
                Verdict.HasEnough or Verdict.MoreThanEnough or Verdict.HasAny => -cost,
                Verdict.HasRoomForMore or Verdict.HasEnoughRoom or Verdict.MoreRoomThanNecessary => cost,
                var _ => 0
            };
            
            KeyValuePair<GameplayAttributeType, Modifier> modifier = new KeyValuePair<GameplayAttributeType, Modifier>(
                this.CostAttribute, new Modifier(ModifierType.Offset, change)
            );
            
            return new[] { modifier };
        }
    }
}
