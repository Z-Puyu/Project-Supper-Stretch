#if UNITY_EDITOR || LiveScriptReload_Enabled
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Project.External.FastScriptReload.Scripts.Runtime
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class DetourCrashHandler
    {
        //TODO: add device support / android crashes / how to report issues back?
        public static string LastDetourFilePath;
    
        static DetourCrashHandler()
        {
#if UNITY_EDITOR
            DetourCrashHandler.Init();
#else
            LoggerScoped.Log($"{nameof(DetourCrashHandler)}: currently only supported in Editor");
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Init()
        {
#if UNITY_EDITOR
            DetourCrashHandler.LastDetourFilePath = Path.GetTempPath() + Application.productName + "-last-detour.txt";
            foreach (var c in Path.GetInvalidFileNameChars()) 
            { 
                DetourCrashHandler.LastDetourFilePath = DetourCrashHandler.LastDetourFilePath.Replace(c, '-'); 
            }
#else
            LoggerScoped.Log($"{nameof(DetourCrashHandler)}: currently only supported in Editor");
#endif
        }

        public static void LogDetour(string fullName)
        {
#if UNITY_EDITOR
            File.AppendAllText(DetourCrashHandler.LastDetourFilePath, fullName + Environment.NewLine);
#else
            LoggerScoped.Log($"{nameof(DetourCrashHandler)}: currently only supported in Editor");
#endif
        }

        public static string RetrieveLastDetour()
        {
#if UNITY_EDITOR
            if (File.Exists(DetourCrashHandler.LastDetourFilePath))
            {
                var lines = File.ReadAllLines(DetourCrashHandler.LastDetourFilePath);
                return lines.Length > 0 ? lines.Last() : string.Empty;
            }

            return string.Empty;
#else
            LoggerScoped.Log($"{nameof(DetourCrashHandler)}: currently only supported in Editor");
            return string.Empty;
#endif
        }

        public static void ClearDetourLog()
        {
#if UNITY_EDITOR
            File.Delete(DetourCrashHandler.LastDetourFilePath);
#else
            LoggerScoped.Log($"{nameof(DetourCrashHandler)}: currently only supported in Editor");
#endif
        }
    }
}
#endif