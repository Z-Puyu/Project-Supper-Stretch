#if UNITY_EDITOR || LiveScriptReload_Enabled
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Project.External.FastScriptReload.Scripts.Runtime
{
    public class ProjectTypeCache
    {
        private static bool _isInitialized;
        private static Dictionary<string, Type> _allTypesInNonDynamicGeneratedAssemblies;
        public static Dictionary<string, Type> AllTypesInNonDynamicGeneratedAssemblies
        {
            get
            {
                if (!ProjectTypeCache._isInitialized)
                {
                    ProjectTypeCache.Init();
                }

                return ProjectTypeCache._allTypesInNonDynamicGeneratedAssemblies;
            }
        }

        private static void Init()
        {
            if (ProjectTypeCache._allTypesInNonDynamicGeneratedAssemblies == null)
            {
                var typeLookupSw = new Stopwatch();
                typeLookupSw.Start();

                ProjectTypeCache._allTypesInNonDynamicGeneratedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                                                                     .Where(a => !CustomAttributeExtensions.GetCustomAttributes<DynamicallyCreatedAssemblyAttribute>((Assembly)a).Any())
                                                                                     .SelectMany(a => a.GetTypes())
                                                                                     .GroupBy(t => t.FullName)
                                                                                     .Select(g => g.First()) //TODO: quite odd that same type full name can be defined multiple times? eg Microsoft.CodeAnalysis.EmbeddedAttribute throws 'An item with the same key has already been added' 
                                                                                     .ToDictionary(t => t.FullName, t => t);
                    
#if ImmersiveVrTools_DebugEnabled
                ImmersiveVrToolsCommon.Runtime.Logging.LoggerScoped.Log($"Initialized type-lookup dictionary, took: {typeLookupSw.ElapsedMilliseconds}ms - cached");
#endif
            }
        }

    }
}
#endif