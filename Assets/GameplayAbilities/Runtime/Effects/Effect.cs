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
        [field: SerializeReference, SubtypeSelector] internal IScheduler? ExecutionScheduler { get; private set; }

        [field: SerializeField]
        private EffectModifierPreset ModifierPreset { get; set; } = new EffectModifierPreset();

        Awaitable IEffect.Execute(
            EffectExecutionContext context, ModifierEnvironment target,
            IUserData? userData, CancellationToken interrupt
        ) {
            KeyValuePair<GameplayAttributeType, Modifier>[] modifiers = this.ModifierPreset.Apply(
                context.SourceAttributes, context.TargetAttributes, userData
            ).ToArray();

            return (this.ExecutionScheduler?.Clone(modifiers) ?? InstantExecution.Create(modifiers)).Execute(
                target, interrupt
            );
        }
    }
}
