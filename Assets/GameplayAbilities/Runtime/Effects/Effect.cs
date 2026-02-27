using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameplayAbilities.Abilities;
using GameplayAbilities.Attributes;
using GameplayAbilities.Modifiers;
using UnityEngine;

namespace GameplayAbilities.Effects {
    [CreateAssetMenu(fileName = "New Effect", menuName = "Gameplay Abilities/Effect")]
    public sealed class Effect : ScriptableObject {
        internal enum Type { Instant, Periodic, Persistent }

        [field: SerializeField] internal Type Periodicity { get; private set; } = Type.Instant;

        [field: SerializeField]
        [field: Tooltip("Whether the effect should run forever until explicitly removed?")]
        internal bool IsInfinite { get; private set; }

        [field: SerializeField]
        private float Duration { get; set; }

        [field: SerializeField]
        private int PeriodCount { get; set; } = 1;

        [field: SerializeField]
        [field: Tooltip("If 0, the effect applies once per frame and modifiers are interpreted as per-second values")]
        private float Interval { get; set; }

        [field: SerializeField]
        private bool ShouldExecuteBeforeFirstInterval { get; set; }

        [field: SerializeField]
        private EffectModifierPreset ModifierPreset { get; set; } = new EffectModifierPreset();

        [field: SerializeField]
        private List<EffectDescriptor> TargetRemovesEffects { get; set; } = new List<EffectDescriptor>();

        private bool IsFinite => !this.IsInfinite;
        private bool IsInstant => this.Periodicity == Type.Instant;
        private bool IsPeriodic => this.Periodicity == Type.Periodic;
        private bool IsContinuous => this.Periodicity == Type.Persistent;

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

                KeyValuePair<GameplayAttributeType, Modifier>[] modifiers =
                        this.ModifierPreset.Apply(source, target, userData).ToArray();
                if (this.Periodicity != Type.Periodic || this.ShouldExecuteBeforeFirstInterval) {
                    foreach ((GameplayAttributeType t, Modifier modifier) in modifiers) {
                        target.AddModifier(t, modifier);
                    }
                } else {
                    CancellationToken interrupt = target.Register(new EffectDescriptor(this, sourceAbility));
                    try {
                        await this.RunAsynchronously(target, modifiers, interrupt);
                    } catch (OperationCanceledException) {

                    }
                }
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        private async Awaitable RunAsynchronously(
            IEffectReceiverFacade target, KeyValuePair<GameplayAttributeType, Modifier>[] modifiers,
            CancellationToken interrupt
        ) {
            switch (this.Periodicity) {
                case Type.Persistent:
                    try {
                        await Awaitable.WaitForSecondsAsync(this.Duration, interrupt);
                    } finally {
                        foreach ((GameplayAttributeType t, Modifier modifier) in modifiers) {
                            target.AddModifier(t, -modifier);
                        }
                    }

                    break;
                case Type.Periodic:
                    int period = 0;
                    while (this.PeriodCount < 0 || period < this.PeriodCount) {
                        if (this.Interval <= 0) {
                            while (!interrupt.IsCancellationRequested) {
                                await Awaitable.NextFrameAsync(interrupt);
                                foreach ((GameplayAttributeType t, Modifier modifier) in modifiers) {
                                    target.AddModifier(t, modifier * Time.deltaTime);
                                }
                            }
                        } else {
                            await Awaitable.WaitForSecondsAsync(this.Interval, interrupt);
                            foreach ((GameplayAttributeType t, Modifier modifier) in modifiers) {
                                target.AddModifier(t, modifier);
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
