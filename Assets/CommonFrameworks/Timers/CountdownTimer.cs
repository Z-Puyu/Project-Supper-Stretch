using System;
using SaintsField;
using UnityEngine;

namespace CommonFrameworks.Timers {
    [Serializable]
    public sealed class CountdownTimer : Timer {
        private double RemainingTime { get; set; }
        [field: SerializeField, MinValue(0)] private double Duration { get; set; }
        [field: SerializeField] private bool IsOneShot { get; set; }
        
        public event Action OnTimeOut = delegate { };
        
        public override double Progress => 1 - this.RemainingTime / this.Duration;
        
        private CountdownTimer() { }

        public CountdownTimer(double duration, bool isOneShot = false) {
            this.Duration = duration;
            this.IsOneShot = isOneShot;
        }

        public override void Tick() {
            this.RemainingTime -= Time.deltaTime;
            if (this.RemainingTime > 0) {
                return;
            }

            this.OnTimeOut.Invoke();
            if (this.IsOneShot) {
                this.Stop();
            } else {
                this.Reset();
            }
        }

        public override Timer Reset() {
            this.RemainingTime = this.Duration;
            return base.Reset();
        }
        
        public CountdownTimer Reset(double duration) {
            this.RemainingTime = duration;
            this.Reset();
            return this;
        }
    }
}
