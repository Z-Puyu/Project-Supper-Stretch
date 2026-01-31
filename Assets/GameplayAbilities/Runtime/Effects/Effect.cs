using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameplayAbilities.Abilities;
using GameplayAbilities.Modifiers;
using GameplayKeywords;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilities.Effects {
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
        [field: Tooltip("If 0, the effect applies once per frame and modifiers are interpreted as per-second values")]
        private float Interval { get; set; }

        [field: SerializeField, ShowIf(nameof(this.IsPeriodic))]
        private bool ShouldExecuteBeforeFirstInterval { get; set; }

        [field: SerializeField] private EffectKeywordPreset KeywordPreset { get; set; } = new EffectKeywordPreset();

        [field: SerializeField, SaintsRow(true)]
        private EffectModifierPreset ModifierPreset { get; set; } = new EffectModifierPreset();

        [field: SerializeField, Table]
        private List<EffectDescriptor> TargetRemovesEffects { get; set; } = new List<EffectDescriptor>();

        private bool IsFinite => !this.IsInfinite;
        private bool IsInstant => this.Periodicity == Type.Instant;
        private bool IsPeriodic => this.Periodicity == Type.Periodic;
        private bool IsContinuous => this.Periodicity == Type.Persistent;
        private AdvancedDropdownList<string> AllKeywords => KeywordUtils.FetchLeaves<EffectTagSheet>();

        /// <summary>
        /// Applies the effect.
        /// </summary>
        /// <param name="source">The instigator of the effect</param>
        /// <param name="target">The target of the effect</param>
        /// <param name="sourceAbility">Optional ability that caused the effect.</param>
        /// <param name="userData">Optional user data for the effect.</param>
        public async void Apply(
            IEffectEmitterFacade source, IEffectReceiverFacade target,
            IReadOnlyDictionary<string, double>? userData = null,
            Ability? sourceAbility = null
        ) {
            try {
                foreach (EffectDescriptor descriptor in this.TargetRemovesEffects) {
                    target.StopEffects(descriptor);
                }

                Modifier[] modifiers = this.ModifierPreset.Apply(source, target, userData).ToArray();
                this.KeywordPreset.Apply(source, target);
                if (this.Periodicity != Type.Periodic || this.ShouldExecuteBeforeFirstInterval) {
                    foreach (Modifier modifier in modifiers) {
                        target.AddModifier(modifier);
                    }
                } else {
                    CancellationToken interrupt = target.Register(new EffectDescriptor(this, sourceAbility));
                    try {
                        await this.RunAsynchronously(target, modifiers, interrupt);
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
                            target.AddModifier(-modifier);
                        }
                    }

                    break;
                case Type.Periodic:
                    int period = 0;
                    while (this.PeriodCount < 0 || period < this.PeriodCount) {
                        if (this.Interval <= 0) {
                            while (!interrupt.IsCancellationRequested) {
                                await Awaitable.NextFrameAsync(interrupt);
                                foreach (Modifier modifier in modifiers) {
                                    target.AddModifier(modifier * Time.deltaTime);
                                }
                            }
                        } else {
                            await Awaitable.WaitForSecondsAsync(this.Interval, interrupt);
                            foreach (Modifier modifier in modifiers) {
                                target.AddModifier(modifier);
                            }
                        }

                        if (this.PeriodCount >= 0) {
                            period += 1;
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
