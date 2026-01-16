using CommonFrameworks.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace CommonFrameworks.Timers {
    internal static class TimerBootstrapper {
        private static PlayerLoopSystem Subsystem { get; set; }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Init() {
            TimerBootstrapper.Subsystem = new PlayerLoopSystem {
                type = typeof(TimerSubsystem),
                updateDelegate = TimerSubsystem.Update,
                subSystemList = null
            };

            PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
            if (!loop.InsertSubsystem<Update>(TimerBootstrapper.Subsystem)) {
                Debug.LogError("Failed to insert timer subsystem");
            } else {
                PlayerLoop.SetPlayerLoop(loop);
            }
            
            loop = PlayerLoop.GetCurrentPlayerLoop();
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= handlePlayModeStateChange;
            EditorApplication.playModeStateChanged += handlePlayModeStateChange;
#endif

            static void handlePlayModeStateChange(PlayModeStateChange state) {
                if (state != PlayModeStateChange.ExitingPlayMode) {
                    return;
                }

                PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
                loop.RemoveSubsystem<Update>(TimerBootstrapper.Subsystem);
                PlayerLoop.SetPlayerLoop(loop);
                TimerSubsystem.Clear();
            }
        }
    }
}
