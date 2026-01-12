using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommonFrameworks.Components;
using GameplayAbilitiesSystem.Runtime.Abilities;
using GameplayAbilitiesSystem.Runtime.Attributes;
using GameplayAbilitiesSystem.Runtime.Effects;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Stats {
    internal sealed class VitalStatistics : BehaviourComponent {
        [NotNull] private AttributeSet? AttributeSet { get; set; }
        [NotNull] private IEffectReceiverFacade? EffectReceiver { get; set; }
        [NotNull] private IEffectEmitterFacade? EffectEmitter { get; set; }
        [field: SerializeField] private List<VitalStat> Stats { get; set; } = new List<VitalStat>();
        
        protected override void Awake() {
            base.Awake();
            this.AttributeSet = this.Root.GetOrAdd<AttributeSet>();
            this.EffectReceiver = this.Root.HasComponent(out IEffectReceiverFacade? facade)
                    ? facade
                    : this.Root.Add<AbilitySystem>();
            this.EffectEmitter = this.Root.HasComponent(out IEffectEmitterFacade? emitter)
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
