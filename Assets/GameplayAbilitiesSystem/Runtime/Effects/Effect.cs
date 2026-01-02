using System.Collections.Generic;
using CommonFrameworks.Utilities;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Pool;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    [CreateAssetMenu(fileName = "New Effect", menuName = "Gameplay Abilities/Effect")]
    public sealed class Effect : ScriptableObject {
        internal enum Type { Instant, Periodic, Persistent }
        
        private static IObjectPool<PeriodicEffect> PeriodicEffectPool { get; } = new ObjectPool<PeriodicEffect>(
            createFunc: () => new PeriodicEffect(), 
            actionOnGet: effect => effect.Reset(),
            actionOnRelease: effect => effect.Stop(), 
            actionOnDestroy: effect => effect.Stop(),
            defaultCapacity: 100
        );

        private static IObjectPool<PersistentEffect> PersistentEffectPool { get; } = new ObjectPool<PersistentEffect>(
            createFunc: () => new PersistentEffect(),
            actionOnGet: effect => effect.Reset(),
            actionOnRelease: effect => effect.Stop(), 
            actionOnDestroy: effect => effect.Stop(),
            defaultCapacity: 100
        );

        [field: LayoutStart("Effect Info", ELayout.Foldout)]
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))]
        internal string Tag { get; private set; } = string.Empty;

        [field: SerializeField] internal Type Periodicity { get; private set; } = Type.Instant;

        [field: SerializeField, HideIf(nameof(this.IsInstant))]
        [field: Tooltip("Whether the effect should run forever until explicitly removed?")]
        internal bool IsInfinite { get; private set; }

        [field: SerializeField, MinValue(0), EndText("seconds")]
        [field: ReadOnly(nameof(this.IsPeriodic)), HideIf(nameof(this.IsInstant), nameof(this.IsInfinite))]
        private float Duration { get; set; }

        [field: SerializeField, MinValue(1), EndText("ticks")]
        [field: ShowIf(nameof(this.IsPeriodic), nameof(this.IsFinite))]
        private int PeriodCount { get; set; } = 1;

        [field: SerializeField, MinValue(0), EndText("seconds"), ShowIf(nameof(this.IsPeriodic))]
        private float Interval { get; set; }

        [field: SerializeField, ShowIf(nameof(this.IsPeriodic))]
        private bool ShouldExecuteBeforeFirstInterval { get; set; }

        [field: SerializeField, Table, LayoutEnd("Effect Info"), LayoutStart("Effect Behaviours", ELayout.Foldout)]
        private List<EffectModifier> Modifiers { get; set; } = new List<EffectModifier>();
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))]
        private List<string> TargetReceivesKeywords { get; set; } = new List<string>();
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))]
        private List<string> TargetRemovesKeywords { get; set; } = new List<string>();
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))]
        private List<string> SourceReceivesKeywords { get; set; } = new List<string>();
        
        [field: SerializeField, TreeDropdown(nameof(this.AllKeywords))]
        private List<string> SourceRemovesKeywords { get; set; } = new List<string>();

        [field: SerializeReference, ReferencePicker]
        private List<IEffect<IEffectEmitterFacade>> CustomSideEffects { get; set; } =
            new List<IEffect<IEffectEmitterFacade>>();
        
        private bool IsFinite => !this.IsInfinite;
        private bool IsInstant => this.Periodicity == Type.Instant;
        private bool IsPeriodic => this.Periodicity == Type.Periodic;
        private bool IsContinuous => this.Periodicity == Type.Persistent;
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.GetTreeDropdownList();

        /// <summary>
        /// Applies the effect.
        /// </summary>
        /// <param name="source">The instigator of the effect</param>
        /// <param name="target">The target of the effect</param>
        /// <param name="continuousEffect">The running effect, if the effect should be active for a duration.</param>
        /// <param name="userData">Optional user data for the effect.</param>
        internal void Apply(
            IEffectEmitterFacade source, IEffectReceiverFacade target, 
            out ContinuousEffect? continuousEffect,
            IReadOnlyDictionary<string, double>? userData = null
        ) {
            List<Modifier> modifiers = new List<Modifier>();
            continuousEffect = null;
            switch (this.Periodicity) {
                case Type.Instant:
                    new InstantEffect(target, null).Apply();
                    break;
                case Type.Periodic:
                    PeriodicEffect periodicEffect = Effect.PeriodicEffectPool.Get();
                    periodicEffect.SourceEffect = this;
                    continuousEffect = periodicEffect;
                    periodicEffect.Apply(
                        new PeriodicEffect.Arguments(
                            target, this.Interval, this.PeriodCount, this.ShouldExecuteBeforeFirstInterval,
                            modifiers, this.TargetReceivesKeywords, this.TargetRemovesKeywords
                        )
                    );
                    break;
                case Type.Persistent:
                    PersistentEffect persistentEffect = Effect.PersistentEffectPool.Get();
                    persistentEffect.SourceEffect = this;
                    continuousEffect = persistentEffect;
                    persistentEffect.Apply(
                        new PersistentEffect.Arguments(
                            target, this.Duration, modifiers, this.TargetReceivesKeywords, this.TargetRemovesKeywords
                        )
                    );
                    break;
            }
        }

        private void OnValidate() {
            if (this.IsPeriodic) {
                if (this.IsInfinite) {
                    this.Duration = -1;
                    this.PeriodCount = -1;
                } else {
                    this.Duration = this.Interval * this.PeriodCount;
                }
            } else if (this.IsContinuous && this.IsInfinite) {
                this.Duration = -1;
            }
        }
    }
}