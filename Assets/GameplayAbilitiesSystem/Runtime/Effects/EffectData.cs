using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Effects.CustomBehaviours;
using GameplayAbilitiesSystem.Runtime.Modifiers;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    [Serializable]
    internal sealed class EffectData : ConditionalExecution {
        internal enum Type { Instant, Periodic, Continuous }

        [field: LayoutStart("Effect Info", ELayout.Foldout)]
        [field: SerializeField, TreeDropdown(nameof(this.GetAllKeywords))]
        private string Tag { get; set; }

        [field: SerializeField] internal Type Periodicity { get; private set; } = Type.Instant;

        [field: SerializeField, HideIf(nameof(this.IsInstant))]
        internal bool IsInfinite { get; private set; }

        [field: SerializeField, MinValue(0), PostFieldRichLabel("seconds")]
        [field: DisableIf(nameof(this.IsPeriodic)), HideIf(nameof(this.IsInstant), nameof(this.IsInfinite))]
        private double Duration { get; set; }

        [field: SerializeField, MinValue(1), PostFieldRichLabel("ticks")]
        [field: ShowIf(nameof(this.IsPeriodic)), HideIf(nameof(this.IsInfinite))]
        private int PeriodCount { get; set; } = 1;

        [field: SerializeField, MinValue(0), PostFieldRichLabel("seconds"), ShowIf(nameof(this.IsPeriodic))]
        private double Interval { get; set; }

        [field: SerializeField, ShowIf(nameof(this.IsPeriodic))]
        private bool ShouldExecuteBeforeFirstInterval { get; set; }

        [field: SerializeField, Table, LayoutEnd("Effect Info"), LayoutStart("Effect Behaviours", ELayout.Foldout)]
        private List<EffectModifier> Modifiers { get; set; } = new List<EffectModifier>();

        [field: SerializeReference, ReferencePicker]
        private List<EffectBehaviour> CustomBehaviours { get; set; } = new List<EffectBehaviour>();
        
        private bool IsInstant => this.Periodicity == Type.Instant;
        private bool IsPeriodic => this.Periodicity == Type.Periodic;
        private bool IsContinuous => this.Periodicity == Type.Continuous;

        private AdvancedDropdownList<string> GetAllKeywords() {
            return AttributeUtils.GetDropdownList();
        }

        internal Effect Instantiate(IEffectEmitterFacade source, IEffectReceiverFacade target) {
            List<Modifier> modifiers = this.Modifiers.Where(modifier => modifier.IsApplicable(source, target))
                                           .ToList().ConvertAll(modifier => modifier.CreateModifier(source, target));
            List<EffectBehaviour> behaviours = this.CustomBehaviours.Where(behaviour =>
                    behaviour is not null && behaviour.IsApplicable(source, target)
            ).ToList();

            return this.Periodicity switch {
                Type.Instant => new InstantEffect(this, target, execute, null),
                Type.Periodic => new PeriodicEffect(
                    this, target, execute, null,
                    this.Duration, (float)this.Interval, this.PeriodCount, this.ShouldExecuteBeforeFirstInterval
                ),
                Type.Continuous => new ContinuousEffect(this, target, execute, stop, this.Duration),
                var _ => throw new ArgumentOutOfRangeException(nameof(this.Periodicity), this.Periodicity, string.Empty)
            };

            void execute() {
                modifiers.ForEach(target.AddModifier);
                behaviours.ForEach(behaviour => behaviour.Apply(target));
            }

            void stop() {
                modifiers.ConvertAll(modifier => new Modifier(modifier.Target, modifier.Type, -modifier.Value))
                         .ForEach(target.AddModifier);
                behaviours.ForEach(behaviour => behaviour.Stop());
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