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

        private IAttributeReader? RegenSource { get; set; }
        private EffectReceiver? RegenReceiver { get; set; }
        
        internal void Watch(AttributeSet set, IAttributeReader source, EffectReceiver receiver) {
            if (this.TrackedAttribute) {
                set.Observe(this.TrackedAttribute, this.React);
            }

            this.RegenReceiver = receiver;
            this.RegenSource = source;
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
            if (change.IsNegligible || this.RegenReceiver == null || change >= 0 || !this.RegenerationEffect) {
                return;
            }

            this.RegenReceiver.Stop(this.RegenerationEffect);
            if (this.RegenerationDelay > 0) {
                this.WaitAndRegenerate();
            } else {
                this.Regenerate();
            }
        }

        private void Regenerate() {
            if (this.RegenerationEffect && this.RegenReceiver != null && this.RegenSource != null) {
                this.RegenReceiver.AddEffect(this.RegenSource, this.RegenerationEffect);
            }
        }
    }
}
