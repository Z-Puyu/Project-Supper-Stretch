using System.Collections.Generic;

namespace CommonFrameworks.Timers {
    public static class TimerSubsystem {
        private static readonly HashSet<Timer> Timers = new HashSet<Timer>();
        private static readonly List<Timer> Sweep = new List<Timer>();

        public static void Register(Timer timer) {
            TimerSubsystem.Timers.Add(timer);
        }

        public static void Deregister(Timer timer) {
            TimerSubsystem.Timers.Remove(timer);
        }

        public static void Update() {
            if (TimerSubsystem.Timers.Count == 0) {
                return;
            }

            TimerSubsystem.Sweep.Clear();
            TimerSubsystem.Sweep.AddRange(TimerSubsystem.Timers);
            foreach (Timer timer in TimerSubsystem.Sweep) {
                if (!timer.IsRunning) {
                    continue;
                }
                
                timer.Tick();
            }
        }
        
        public static void Clear() {
            TimerSubsystem.Sweep.Clear();
            TimerSubsystem.Sweep.AddRange(TimerSubsystem.Timers);
            foreach (Timer timer in TimerSubsystem.Sweep) {
                timer.Dispose();
            }
            
            TimerSubsystem.Timers.Clear();
            TimerSubsystem.Sweep.Clear();
        }
    }
}
