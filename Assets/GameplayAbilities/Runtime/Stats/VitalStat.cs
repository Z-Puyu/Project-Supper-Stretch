using System;
using GameplayAbilities.Attributes;
using GameplayAbilities.Effects;
using UnityEngine;

namespace GameplayAbilities.Stats {
    [Serializable]
    public sealed class VitalStat {
        [field: SerializeField] private GameplayAttributeType? TrackedAttribute { get; set; }
        [field: SerializeField] private Effect? RegenerationEffect { get; set; }
        [field: SerializeField] private float RegenerationDelay { get; set; }

        private IEffectEmitterFacade? RegenEmitter { get; set; }
        private IEffectReceiverFacade? RegenReceiver { get; set; }
        
        internal void Watch(AttributeSet set, IEffectEmitterFacade emitter, IEffectReceiverFacade receiver) {
            if (this.TrackedAttribute) {
                set.Observe(this.TrackedAttribute, this.React);
            }

            this.RegenReceiver = receiver;
            this.RegenEmitter = emitter;
        }
        
        private async void WaitAndRegenerate() {
            try {
                await Awaitable.WaitForSecondsAsync(this.RegenerationDelay);
                this.Regenerate();
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        private void React(GameplayAttributeType _, AttributeChange change) {
            if (change.IsNegligible || this.RegenReceiver == null || change >= 0) {
                return;
            }

            this.RegenReceiver.StopEffects(new EffectDescriptor(this.RegenerationEffect));
            if (this.RegenerationDelay > 0) {
                this.WaitAndRegenerate();
            } else {
                this.Regenerate();
            }
        }

        private void Regenerate() {
            if (this.RegenerationEffect && this.RegenReceiver != null && this.RegenEmitter != null) {
                this.RegenerationEffect.Apply(this.RegenEmitter, this.RegenReceiver);
            }
        }
    }
}
