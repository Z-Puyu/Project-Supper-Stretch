using ImmersiveVrToolsCommon.Runtime.Logging;
using UnityEngine;

namespace Project.External.FastScriptReload.Scripts.Runtime
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class LoggerScopedInitializer
    {
        static LoggerScopedInitializer()
        {
            LoggerScopedInitializer.Init();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Init()
        {
            LoggerScoped.LogPrefix = "FSR: ";
        }
    }
}