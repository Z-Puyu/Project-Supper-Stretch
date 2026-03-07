using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameplayAbilities.Attributes;
using GameplayAbilities.Common;
using GameplayAbilities.Effects.Schedulers;
using GameplayAbilities.Effects.Stacking;
using GameplayAbilities.Modifiers;
using GameplayAbilities.Runtime.EditorTooling;
using UnityEngine;

namespace GameplayAbilities.Effects {
    [CreateAssetMenu(fileName = "New Effect", menuName = "Gameplay Abilities/Effect")]
    public sealed class Effect : ScriptableObject, IEffect {
        [field: SerializeReference, SubtypeSelector] internal IScheduler? ExecutionScheduler { get; private set; }
        [field: SerializeField] private List<ModifierConfig> Modifiers { get; set; } = new List<ModifierConfig>();
        [field: SerializeField, Min(1)] private int StackingLimit { get; set; } = 1;
        [field: SerializeReference, SubtypeSelector] internal IEffectStacker? StackingScheme { get; private set; }
        internal int StackLimit => Math.Max(this.StackingLimit, 1);

        internal EffectStackingResult StackWith(
            EffectExecutionState existing, EffectExecutionContext context, IUserData? userData
        ) {
            return this.StackingScheme is null
                    ? EffectStackingResult.DirectStackingOf(this.CreateExecution(context, userData))
                    : this.StackingScheme.Stack(existing, this.CreateExecution(context, userData));
        }

        internal EffectExecutionScheme CreateExecution(EffectExecutionContext context, IUserData? userData) {
            return new EffectExecutionScheme {
                Modifiers = this.MakeModifiers(context, userData),
                ExecutionSchedule = this.ExecutionScheduler?.ExecutionSchedule ?? default
            };
        }

        RuntimeEffect IEffect.Execute(
            EffectExecutionScheme scheme, ModifierEnvironment target, CancellationTokenSource interrupter
        ) {
            IScheduler scheduler = this.ExecutionScheduler?.Schedule(scheme) ??
                                   InstantExecution.Create(scheme.Modifiers);
            return RuntimeEffect.With(this, scheduler, interrupter, target);
        }

        EffectExecutionScheme IEffect.CreateExecutionScheme(EffectExecutionContext context, IUserData? userData) {
            return this.CreateExecution(context, userData);
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
