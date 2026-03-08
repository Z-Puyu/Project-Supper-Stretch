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


        [field: SerializeReference, SubtypeSelector] internal IScheduler? ExecutionScheduler { get; private set; }
        [field: SerializeField] private List<ModifierConfig> Modifiers { get; set; } = new List<ModifierConfig>();
        [field: SerializeField, Min(1)] private int StackingLimit { get; set; } = 1;
        [field: SerializeField] private StackingRule StackingPolicy { get; set; } = StackingRule.Independent;
        internal int StackLimit => Math.Max(this.StackingLimit, 1);

        internal EffectStackingResult StackWith(
            EffectExecutionState existing, EffectExecutionContext context, IUserData? userData
        ) {
            EffectExecutionScheme @new = this.CreateExecutionScheme(context, userData);
            return new EffectStackingResult {
                NewEffectExecutionScheme = this.StackingPolicy switch {
                    StackingRule.Independent => @new,
                    StackingRule.Extend => @new with {
                        StackSize = existing.StackSize + 1,
                        ExecutionSchedule = (existing + @new.ExecutionSchedule) with { ShouldTickOnStart = true }
                    },
                    StackingRule.MergeAndExtend => new EffectExecutionScheme(
                        @new.Modifiers.Concat(existing.Modifiers),
                        (existing + @new.ExecutionSchedule) with { ShouldTickOnStart = true },
                        existing.StackSize + 1
                    ),
                    StackingRule.MergeAndOverride => @new with {
                        StackSize = existing.StackSize + 1,
                        Modifiers = existing.Modifiers.Concat(@new.Modifiers)
                    },
                    var _ => throw new ArgumentOutOfRangeException(nameof(this.StackingPolicy))
                }
            };
        }

        RuntimeEffect IEffect.Execute(
            EffectExecutionScheme scheme, ModifierEnvironment target, CancellationTokenSource interrupter
        ) {
            IScheduler scheduler = this.ExecutionScheduler?.Schedule(scheme) ??
                                   InstantExecution.Create(scheme.Modifiers);
            return RuntimeEffect.With(this, scheduler, interrupter, target);
        }

        internal EffectExecutionScheme CreateExecutionScheme(EffectExecutionContext context, IUserData? userData) {
            return new EffectExecutionScheme {
                StackSize = 1,
                Modifiers = this.MakeModifiers(context, userData),
                ExecutionSchedule = this.ExecutionScheduler?.ExecutionSchedule ?? default
            };
        }

        private IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> MakeModifiers(
            EffectExecutionContext context, IUserData? userData
        ) {
            foreach (ModifierConfig config in this.Modifiers) {
                if (!config.Target) {
                    continue;
                }
                
                Modifier modifier = config.Instantiate(context.SourceAttributes, context.TargetAttributes, userData);
                foreach (GameplayAttributeType t in config.Target.Resolve()) {
                    yield return new KeyValuePair<GameplayAttributeType, Modifier>(t, modifier);
                }
            }
        }
    }
}
