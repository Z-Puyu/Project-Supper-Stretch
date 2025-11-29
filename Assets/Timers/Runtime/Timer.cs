using System;

namespace Timers.Runtime {
    public abstract class Timer : IDisposable {
        private bool disposed;
        public bool IsRunning { get; private set; }
        public bool IsFinished { get; protected set; }

        public abstract double Progress { get; }

        public event Action OnStart;
        public event Action OnStop;

        public void Start() {
            if (this.IsRunning) {
                return;
            }

            this.IsRunning = true;
            TimerSubsystem.Register(this);
            this.OnStart?.Invoke();
        }

        public void Stop() {
            if (!this.IsRunning) {
                return;
            }

            this.IsRunning = false;
            TimerSubsystem.Deregister(this);
            this.OnStop?.Invoke();
        }

        public abstract void Tick();

        public void Resume() {
            this.IsRunning = true;
        }

        public void Pause() {
            this.IsRunning = false;
        }

        public abstract Timer Reset();

        ~Timer() {
            this.Dispose(false);
        }
        
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (this.disposed) {
                return;
            }

            if (disposing) {
                TimerSubsystem.Deregister(this);
            }

            this.disposed = true;
        }
    }
}
