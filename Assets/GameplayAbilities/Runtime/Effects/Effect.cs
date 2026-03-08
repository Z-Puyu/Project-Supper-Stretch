using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Common;
using GameplayAbilities.Effects.Schedulers;
using GameplayAbilities.Modifiers;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Effects {
    [CreateAssetMenu(fileName = "New Effect", menuName = "Gameplay Abilities/Effect")]
    public sealed class Effect : ScriptableObject, IEffect {
        private enum StackingRule {
            Independent,
            [InspectorName("Extend Duration Only")] Extend,
            [InspectorName("Merge Modifiers and Extend Duration")] MergeAndExtend,
            [InspectorName("Merge Modifiers and Reset Duration")] MergeAndOverride
        }
        
        [field: SerializeField] private EffectExecutionPolicy ExecutionPolicy { get; set; }
        [field: SerializeField] private List<ModifierConfig> Modifiers { get; set; } = new List<ModifierConfig>();
        [field: SerializeField, Min(1)] private int StackingLimit { get; set; } = 1;
        [field: SerializeField] private StackingRule StackingPolicy { get; set; } = StackingRule.Independent;
        internal int StackLimit => Math.Max(this.StackingLimit, 1);

        internal RuntimeEffect StackAndExecute(EffectStackingContext context, out StackingResult res) {
            IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> modifiers =
                    this.StackingPolicy == StackingRule.Independent
                            ? this.MakeModifiers(context.NewEffectExecutionContext)
                            : context.CurrentModifiers.Concat(
                                this.MakeModifiers(context.NewEffectExecutionContext)
                            );
            res = new StackingResult {
                OverridesLastExecution = this.StackingPolicy != StackingRule.Independent,
                NewStackSize = this.StackingPolicy == StackingRule.Independent ? 1 : context.CurrentStackSize + 1
            };

            bool resetDuration = this.StackingPolicy is StackingRule.MergeAndOverride or StackingRule.MergeAndExtend;
            EffectExecutionSchedule schedule = this.ExecutionPolicy.Schedule with {
                NumberOfTicks = resetDuration
                        ? this.ExecutionPolicy.Schedule.NumberOfTicks
                        : context.RemainingTicks + this.ExecutionPolicy.Schedule.NumberOfTicks,
                Duration = resetDuration
                        ? this.ExecutionPolicy.Schedule.Duration
                        : context.RemainingDuration + this.ExecutionPolicy.Schedule.Duration,
                WaitingTimeBeforeFirstTick = resetDuration
                        ? this.ExecutionPolicy.Schedule.WaitingTimeBeforeFirstTick
                        : context.WaitingTimeUntilNextTick,
            };
            
            EffectExecutionScheduler executor = this.ExecutionPolicy.CreateScheduler(schedule)
                                                    .Schedule(modifiers, res.NewStackSize);
            return new RuntimeEffect {
                Id = Guid.NewGuid(),
                Source = this,
                Executor = executor,
                Interrupter = context.NewEffectInterrupter,
                Task = executor.Execute(context.CurrentTarget, context.NewEffectInterrupter.Token)
            };
        }

        RuntimeEffect IEffect.Execute(
            EffectExecutionContext context, ModifierEnvironment target, CancellationTokenSource interrupter
        ) {
            EffectExecutionScheduler executor = this.ExecutionPolicy.DefaultScheduler.Schedule(this.MakeModifiers(context));
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
            foreach (ModifierConfig config in this.Modifiers) {
                if (!config.Target) {
                    continue;
                }

                Modifier modifier = config.Instantiate(
                    context.SourceAttributes, context.TargetAttributes, context.UserData
                );
                
                foreach (GameplayAttributeType t in config.Target.Resolve()) {
                    yield return new KeyValuePair<GameplayAttributeType, Modifier>(t, modifier);
                }
            }
        }
    }
}
