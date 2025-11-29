using CommonFrameworks.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Timers.Runtime {
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
            if (!loop.InsertSubsystem<PreUpdate>(TimerBootstrapper.Subsystem, 0)) {
                Debug.LogError("Failed to insert timer subsystem");
            } else {
                PlayerLoop.SetPlayerLoop(loop);
            }
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= handlePlayModeStateChange;
            EditorApplication.playModeStateChanged += handlePlayModeStateChange;
#endif

            static void handlePlayModeStateChange(PlayModeStateChange state) {
                if (state != PlayModeStateChange.ExitingPlayMode) {
                    return;
                }

                PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
                loop.RemoveSubsystem<PreUpdate>(TimerBootstrapper.Subsystem);
                PlayerLoop.SetPlayerLoop(loop);
                TimerSubsystem.Clear();
            }
        }
    }
}
