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
        [field: SerializeField] private List<ModifierConfig> Modifiers { get; set; } = new List<ModifierConfig>();

        Awaitable IEffect.Execute(
            EffectExecutionContext context, ModifierEnvironment target,
            IUserData? userData, CancellationToken interrupt
        ) {
            KeyValuePair<GameplayAttributeType, Modifier>[] modifiers = this.MakeModifiers(
                context.SourceAttributes, context.TargetAttributes, userData
            ).ToArray();

            IScheduler scheduler = this.ExecutionScheduler?.Clone(modifiers) ?? InstantExecution.Create(modifiers);
            return scheduler.Execute(target, interrupt);
        }
        
        private IEnumerable<KeyValuePair<GameplayAttributeType, Modifier>> MakeModifiers(
            IAttributeReader source, IAttributeReader target, IUserData? userData
        ) {
            foreach (ModifierConfig config in this.Modifiers) {
                if (!config.Target) {
                    continue;
                }
                
                Modifier modifier = config.Instantiate(source, target, userData);
                foreach (GameplayAttributeType t in config.Target.Resolve()) {
                    yield return new KeyValuePair<GameplayAttributeType, Modifier>(t, modifier);
                }
            }
        }
    }
}
