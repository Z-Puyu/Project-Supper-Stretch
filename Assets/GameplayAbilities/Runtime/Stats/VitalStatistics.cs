using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Components;
using GameplayAbilities.Abilities;
using GameplayAbilities.Attributes;
using GameplayAbilities.Effects;
using UnityEngine;

namespace GameplayAbilities.Stats {
    [AddComponentMenu("")]
    internal sealed class VitalStatistics : Module {
        [NotNull] private AttributeSet? AttributeSet { get; set; }
        [NotNull] private IEffectReceiverFacade? EffectReceiver { get; set; }
        [NotNull] private IEffectEmitterFacade? EffectEmitter { get; set; }
        [field: SerializeField] private List<VitalStat> Stats { get; set; } = new List<VitalStat>();
        
        protected override void Awake() {
            base.Awake();
            this.AttributeSet = this.Root.GetOrAdd<AttributeSet>();
            this.EffectReceiver = this.Root.HasModule(out IEffectReceiverFacade? facade)
                    ? facade
                    : this.Root.Add<AbilitySystem>();
            this.EffectEmitter = this.Root.HasModule(out IEffectEmitterFacade? emitter)
                    ? emitter
                    : this.Root.Add<AbilitySystem>();
        }

        private void Start() {
            foreach (VitalStat stat in this.Stats) {
                stat.Watch(this.AttributeSet, this.EffectEmitter, this.EffectReceiver);
            }
        }
    }
}
