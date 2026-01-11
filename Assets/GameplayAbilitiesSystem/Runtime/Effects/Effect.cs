using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommonFrameworks.Utilities;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using GameplayKeywordsSystem.Runtime;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    [CreateAssetMenu(fileName = "New Effect", menuName = "Gameplay Abilities/Effect")]
    public sealed class Effect : ScriptableObject {
        internal enum Type { Instant, Periodic, Persistent }
        
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
        
        [field: SerializeField, ShowIf(nameof(this.IsPeriodic))]
        private bool ShouldInterpolateBetweenTicks { get; set; }
        
        [field: SerializeField] private EffectKeywordPreset KeywordPreset { get; set; } = new EffectKeywordPreset();
        
        [field: SerializeField, SaintsRow(true)] 
        private EffectModifierPreset ModifierPreset { get; set; } = new EffectModifierPreset();
        
        private bool IsFinite => !this.IsInfinite;
        private bool IsInstant => this.Periodicity == Type.Instant;
        private bool IsPeriodic => this.Periodicity == Type.Periodic;
        private bool IsContinuous => this.Periodicity == Type.Persistent;
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.Fetch<EffectTagSheet>();

        /// <summary>
        /// Applies the effect.
        /// </summary>
        /// <param name="source">The instigator of the effect</param>
        /// <param name="target">The target of the effect</param>
        /// <param name="sourceAbility">Optional ability that caused the effect.</param>
        /// <param name="userData">Optional user data for the effect.</param>
        /// <param name="interrupt">The cancellation token for interrupting the effect externally.</param>
        public async void Apply(
            IEffectEmitterFacade source, IEffectReceiverFacade target,
            IReadOnlyDictionary<string, double>? userData = null,
            Ability? sourceAbility = null, CancellationToken interrupt = default
        ) {
            try {
                Modifier[] modifiers = this.ModifierPreset.Apply(source, target, userData).ToArray();
                this.KeywordPreset.Apply(source, target);
                if (this.Periodicity != Type.Periodic || this.ShouldExecuteBeforeFirstInterval) {
                    foreach (Modifier modifier in modifiers) {
                        target.ModifierConsumer.AddModifier(modifier);
                    }
                } else {
                    using CancellationTokenSource interrupter = target.Register(
                        new EffectDescriptor(sourceAbility, this, this.Tag), interrupt
                    );

                    try {
                        await this.RunAsynchronously(target, modifiers, interrupter.Token);
                    } catch (OperationCanceledException) {
                        this.KeywordPreset.Revoke(source, target);
                    }
                }
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        private async Awaitable RunAsynchronously(
            IEffectReceiverFacade target, Modifier[] modifiers, CancellationToken interrupt
        ) {
            switch (this.Periodicity) {
                case Type.Persistent:
                    try {
                        await Awaitable.WaitForSecondsAsync(this.Duration, interrupt);
                    } finally {
                        foreach (Modifier modifier in modifiers) {
                            target.ModifierConsumer.AddModifier(-modifier);
                        }
                    }
                    
                    break;
                case Type.Periodic:
                    for (int i = 0; i < this.PeriodCount; i += 1) {
                        if (this.ShouldInterpolateBetweenTicks) {
                            float progress = 0;
                            while (progress < this.Interval) {
                                await Awaitable.NextFrameAsync(interrupt);
                                float delta = Mathf.Min(Time.deltaTime, this.Interval - progress);
                                progress += Time.deltaTime;
                                foreach (Modifier modifier in modifiers) {
                                    target.ModifierConsumer.AddModifier(modifier * (delta / this.Interval));
                                }
                            }
                        } else {
                            await Awaitable.WaitForSecondsAsync(this.Interval, interrupt);
                            foreach (Modifier modifier in modifiers) {
                                target.ModifierConsumer.AddModifier(modifier);
                            }
                        }
                    }
                    
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