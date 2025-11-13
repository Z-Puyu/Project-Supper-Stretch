using System;
using UnityEngine;

namespace Timers.Runtime {
    public class CountdownTimer : Timer {
        private double RemainingTime { get; set; }
        private double Duration { get; set; }
        private bool IsOneShot { get; set; }
        
        public event Action OnTimeOut;
        
        public override double Progress => 1 - this.RemainingTime / this.Duration;

        public CountdownTimer(double duration, bool isOneShot = false) {
            this.Duration = duration;
            this.IsOneShot = isOneShot;
        }

        public override void Tick() {
            this.RemainingTime -= Time.deltaTime;
            if (this.RemainingTime > 0) {
                return;
            }

            this.OnTimeOut?.Invoke();
            if (this.IsOneShot) {
                this.Stop();
            } else {
                this.Reset();
            }
        }

        public override Timer Reset() {
            this.RemainingTime = this.Duration;
            return this;
        }
        
        public CountdownTimer Reset(double duration) {
            this.RemainingTime = duration;
            this.Reset();
            return this;
        }
    }
}
