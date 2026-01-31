using System;
using CommonFrameworks.Timers;
using GameplayAbilities.Attributes;
using GameplayAbilities.Effects;
using SaintsField;
using UnityEngine;

namespace GameplayAbilities.Stats {
    [Serializable]
    public sealed class VitalStat {
        [field: SerializeField, TreeDropdown(nameof(this.AllAttributes))]
        private string TrackedAttribute { get; set; } = string.Empty;

        [field: SerializeField] private Effect? RegenerationEffect { get; set; }

        [field: SerializeField, EndText("seconds"), MinValue(0)]
        private float RegenerationDelay { get; set; }

        private IEffectEmitterFacade? RegenEmitter { get; set; }
        private IEffectReceiverFacade? RegenReceiver { get; set; }
        private CountdownTimer Timer { get; set; } = new CountdownTimer(0, true);

        private AdvancedDropdownList<string> AllAttributes => AttributeUtils.GetLeafAttributes();

        internal void Watch(AttributeSet set, IEffectEmitterFacade emitter, IEffectReceiverFacade receiver) {
            set.Observe(this.TrackedAttribute, this.React);
            this.RegenReceiver = receiver;
            this.RegenEmitter = emitter;
            this.Timer = this.Timer.Reset(this.RegenerationDelay);
            this.Timer.OnTimeOut += this.Regenerate;
        }

        private void React(AttributeKey _, AttributeChange change) {
            if (change.IsNegligible || this.RegenReceiver == null || change >= 0) {
                return;
            }

            this.RegenReceiver.StopEffects(new EffectDescriptor(this.RegenerationEffect));
            if (this.RegenerationDelay > 0) {
                this.Timer.Reset().Start();
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
