﻿#if UNITY_EDITOR || LiveScriptReload_Enabled

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ImmersiveVRTools.Runtime.Common;
using ImmersiveVRTools.Runtime.Common.Extensions;
using ImmersiveVrToolsCommon.Runtime.Logging;
using UnityEngine;

namespace Project.External.FastScriptReload.Scripts.Runtime
{
    [PreventHotReload]
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class AssemblyChangesLoader: IAssemblyChangesLoader
    {
        const BindingFlags ALL_BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic |
                                               BindingFlags.Static | BindingFlags.Instance |
                                               BindingFlags.FlattenHierarchy;
            
        const BindingFlags ALL_DECLARED_METHODS_BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic |
                                                                BindingFlags.Static | BindingFlags.Instance |
                                                                BindingFlags.DeclaredOnly; //only declared methods can be redirected, otherwise it'll result in hang
        
        public const string ClassnamePatchedPostfix = "__Patched_";
        public const string ON_HOT_RELOAD_METHOD_NAME = "OnScriptHotReload";
        public const string ON_HOT_RELOAD_NO_INSTANCE_STATIC_METHOD_NAME = "OnScriptHotReloadNoInstance";

        private static readonly List<Type> ExcludeMethodsDefinedOnTypes = new List<Type>
        {
            typeof(MonoBehaviour),
            typeof(Behaviour),
            typeof(UnityEngine.Object),
            typeof(Component),
            typeof(System.Object)
        }; //TODO: move out and possibly define a way to exclude all non-client created code? as this will crash editor
        
        private static AssemblyChangesLoader _instance;
        public static AssemblyChangesLoader Instance => AssemblyChangesLoader._instance ?? (AssemblyChangesLoader._instance = new AssemblyChangesLoader());

        private Dictionary<Type, Type> _existingTypeToRedirectedType = new Dictionary<Type, Type>();

        public void DynamicallyUpdateMethodsForCreatedAssembly(Assembly dynamicallyLoadedAssemblyWithUpdates, AssemblyChangesLoaderEditorOptionsNeededInBuild editorOptions)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                foreach (var createdType in dynamicallyLoadedAssemblyWithUpdates.GetTypes()
                             .Where(t => (t.IsClass
                                         && !typeof(Delegate).IsAssignableFrom(t)) //don't redirect delegates
                                         // || (t.IsValueType && !t.IsPrimitive) //struct check, ensure works
                             )
                        )
                {
                    if (createdType.GetCustomAttribute<PreventHotReload>() != null)
                    {
                        //TODO: ideally type would be excluded from compilation not just from detour
                        LoggerScoped.Log($"Type: {createdType.Name} marked as {nameof(PreventHotReload)} - ignoring change.");
                        continue;
                    }
                    
                    var createdTypeNameWithoutPatchedPostfix = AssemblyChangesLoader.RemoveClassPostfix(createdType.FullName);
                    if (ProjectTypeCache.AllTypesInNonDynamicGeneratedAssemblies.TryGetValue(createdTypeNameWithoutPatchedPostfix, out var matchingTypeInExistingAssemblies))
                    {
                        this._existingTypeToRedirectedType[matchingTypeInExistingAssemblies] = createdType;
                        
                        if (!editorOptions.IsDidFieldsOrPropertyCountChangedCheckDisabled 
                            && !editorOptions.EnableExperimentalAddedFieldsSupport
                            && AssemblyChangesLoader.DidFieldsOrPropertyCountChanged(createdType,  matchingTypeInExistingAssemblies))
                        {
                            continue;
                        }

                        var allDeclaredMethodsInExistingType = matchingTypeInExistingAssemblies.GetMethods(AssemblyChangesLoader.ALL_DECLARED_METHODS_BINDING_FLAGS)
                            .Where(m => !AssemblyChangesLoader.ExcludeMethodsDefinedOnTypes.Contains(m.DeclaringType))
                            .ToList();
                        foreach (var createdTypeMethodToUpdate in createdType.GetMethods(AssemblyChangesLoader.ALL_DECLARED_METHODS_BINDING_FLAGS)
                                     .Where(m => !AssemblyChangesLoader.ExcludeMethodsDefinedOnTypes.Contains(m.DeclaringType)))
                        {
                            var createdTypeMethodToUpdateFullDescriptionWithoutPatchedClassPostfix = AssemblyChangesLoader.RemoveClassPostfix(createdTypeMethodToUpdate.FullDescription());
                            var matchingMethodInExistingType = allDeclaredMethodsInExistingType
                                .SingleOrDefault(m => m.FullDescription() == createdTypeMethodToUpdateFullDescriptionWithoutPatchedClassPostfix);
                            if (matchingMethodInExistingType != null)
                            {
                                if (matchingMethodInExistingType.IsGenericMethod)
                                {
                                    LoggerScoped.LogWarning($"Method: '{matchingMethodInExistingType.FullDescription()}' is generic. Hot-Reload for generic methods is not supported yet, you won't see changes for that method.");
                                    continue;
                                }

                                if (matchingMethodInExistingType.DeclaringType != null && matchingMethodInExistingType.DeclaringType.IsGenericType)
                                {
                                    LoggerScoped.LogWarning($"Type for method: '{matchingMethodInExistingType.FullDescription()}' is generic. Hot-Reload for generic types is not supported yet, you won't see changes for that type.");
                                    continue;
                                }

                                LoggerScoped.LogDebug($"Trying to detour method, from: '{matchingMethodInExistingType.FullDescription()}' to: '{createdTypeMethodToUpdate.FullDescription()}'");
                                DetourCrashHandler.LogDetour(matchingMethodInExistingType.ResolveFullName());
                                Memory.DetourMethod(matchingMethodInExistingType, createdTypeMethodToUpdate);
                            }
                            else 
                            {
                                LoggerScoped.LogWarning($"Method: {createdTypeMethodToUpdate.FullDescription()} does not exist in initially compiled type: {matchingTypeInExistingAssemblies.FullName}. " +
                                                 $"Adding new methods at runtime is not fully supported. \r\n" +
                                                 $"It'll only work new method is only used by declaring class (eg private method)\r\n" +
                                                 $"Make sure to add method before initial compilation.");
                            }
                        }
                        
                        AssemblyChangesLoader.FindAndExecuteStaticOnScriptHotReloadNoInstance(createdType);
                        AssemblyChangesLoader.FindAndExecuteOnScriptHotReload(matchingTypeInExistingAssemblies, createdType);
                    }
                    else
                    {
                        LoggerScoped.LogWarning($"FSR: Unable to find existing type for: '{createdType.FullName}', this is not an issue if you added new type. <color=orange>If it's an existing type please do a full domain-reload - one of optimisations is to cache existing types for later lookup on first call.</color>");
                        AssemblyChangesLoader.FindAndExecuteStaticOnScriptHotReloadNoInstance(createdType);
                        AssemblyChangesLoader.FindAndExecuteOnScriptHotReload(createdType, createdType);
                    }
                }
                
                LoggerScoped.Log($"Hot-reload completed (took {sw.ElapsedMilliseconds}ms)");
            }
            finally
            {
                DetourCrashHandler.ClearDetourLog();
            }
        }
        
        public Type GetRedirectedType(Type forExistingType)
        {
            return this._existingTypeToRedirectedType[forExistingType];
        }

        private static bool DidFieldsOrPropertyCountChanged(Type createdType, Type matchingTypeInExistingAssemblies)
        {
            var createdTypeFieldAndProperties = createdType.GetFields(AssemblyChangesLoader.ALL_BINDING_FLAGS).Concat(createdType.GetProperties(AssemblyChangesLoader.ALL_BINDING_FLAGS).Cast<MemberInfo>()).ToList();
            var matchingTypeFieldAndProperties = matchingTypeInExistingAssemblies.GetFields(AssemblyChangesLoader.ALL_BINDING_FLAGS).Concat(matchingTypeInExistingAssemblies.GetProperties(AssemblyChangesLoader.ALL_BINDING_FLAGS).Cast<MemberInfo>()).ToList();
            if (createdTypeFieldAndProperties.Count != matchingTypeFieldAndProperties.Count)
            {
                var addedMemberNames = createdTypeFieldAndProperties.Select(m => m.Name).Except(matchingTypeFieldAndProperties.Select(m => m.Name)).ToList();
                LoggerScoped.LogError($"It seems you've added/removed field to changed script. This is not supported and will result in undefined behaviour. Hot-reload will not be performed for type: {matchingTypeInExistingAssemblies.Name}" +
                               $"\r\n\r\nYou can skip the check and force reload anyway if needed, to do so go to: 'Window -> Fast Script Reload -> Start Screen -> Reload -> tick 'Disable added/removed fields check'" +
                               (addedMemberNames.Any() ? $"\r\nAdded: {string.Join(", ", addedMemberNames)}" : ""));
                LoggerScoped.Log(
                    $"<color=orange>There's an experimental feature that allows to add new fields (which are adjustable in editor), to enable please:</color>" +
                    $"\r\n - Open Settings 'Window -> Fast Script Reload -> Start Screen -> New Fields -> tick 'Enable experimental added field support'");
                return true;
            }

            return false;
        }

        private static void FindAndExecuteStaticOnScriptHotReloadNoInstance(Type createdType)
        {
            var onScriptHotReloadStaticFnForType = createdType.GetMethod(AssemblyChangesLoader.ON_HOT_RELOAD_NO_INSTANCE_STATIC_METHOD_NAME,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (onScriptHotReloadStaticFnForType != null)
            {
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    onScriptHotReloadStaticFnForType.Invoke(null, null);
                });
            }
        }

        private static void FindAndExecuteOnScriptHotReload(Type originalType, Type detourType)
        {
            var onScriptHotReloadFnForType = originalType.GetMethod(AssemblyChangesLoader.ON_HOT_RELOAD_METHOD_NAME, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (onScriptHotReloadFnForType != null)
            {
                AssemblyChangesLoader.ExecuteFnOnMainThread(originalType, onScriptHotReloadFnForType);
            }
            else
            { 
                //When OnScriptHotReload method is not present in original type reflection can not use method from new type (as instance types are not matching and will cause exception)
                //creating dynamic method and dotouring that solves the issue
                //On some 2020 Unity versions, eg 2020.3.27f DynamicMethod can not be resolved. Using reflection to ensure it can be compiled and potentially run if methods exist
                
                var onScriptHotReloadFnForCreatedType = detourType.GetMethod(AssemblyChangesLoader.ON_HOT_RELOAD_METHOD_NAME, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (onScriptHotReloadFnForCreatedType != null)
                {
                    //PERF: could potentially cache, negligible overhead
                    var dynamicMethodType = Type.GetType("System.Reflection.Emit.DynamicMethod");
                    if (dynamicMethodType == null)
                    {
                        LoggerScoped.LogWarning($"Unable to find DynamicMethod, added {AssemblyChangesLoader.ON_HOT_RELOAD_METHOD_NAME} won't be called. Make sure to add method before initial compilation.");
                        return;
                    }
                    
                    var dynamicMethodCtor = dynamicMethodType.GetConstructor(new Type[] { typeof(string), typeof(Type), typeof(Type[]) });
                    var dynamicMethodDynamicallyAdded = (MethodInfo)dynamicMethodCtor.Invoke(new object[] { AssemblyChangesLoader.ON_HOT_RELOAD_METHOD_NAME + "_DynamicallyAdded", typeof(void), new Type[] { } });
                
                    var getILGeneratorMethod = dynamicMethodType.GetMethod("GetILGenerator", new Type[] { });
                    var gen = getILGeneratorMethod.Invoke(dynamicMethodDynamicallyAdded, new object[]{ });
                
                    var emitMethod = gen.GetType().GetMethod("Emit", new [] { typeof(OpCode) });
                    emitMethod.Invoke(gen, new object[] { OpCodes.Ret }); //simple return to ensure IL is valid
                    
                    Memory.DetourMethod(dynamicMethodDynamicallyAdded, onScriptHotReloadFnForCreatedType);

                    AssemblyChangesLoader.ExecuteFnOnMainThread(originalType, dynamicMethodDynamicallyAdded);
                }
            }
        }

        private static void ExecuteFnOnMainThread(Type originalType, MethodInfo onScriptHotReloadFn)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                if (!typeof(MonoBehaviour).IsAssignableFrom(originalType))
                {
                    LoggerScoped.LogWarning($"Type: {originalType.Name} is not {nameof(MonoBehaviour)}, {AssemblyChangesLoader.ON_HOT_RELOAD_METHOD_NAME} method can't be executed. You can still use static version: {AssemblyChangesLoader.ON_HOT_RELOAD_NO_INSTANCE_STATIC_METHOD_NAME}");
                    return;
                }

                foreach (var instanceOfType in GameObject.FindObjectsOfType(originalType)) //TODO: perf - could find them in different way?
                {
                    onScriptHotReloadFn.Invoke(instanceOfType, null);
                }
            });
        }

        private static string RemoveClassPostfix(string fqdn)
        {
            return fqdn.Replace(AssemblyChangesLoader.ClassnamePatchedPostfix, string.Empty);
        }
    }
    
    
    [AttributeUsage(AttributeTargets.Assembly)]
    public class DynamicallyCreatedAssemblyAttribute : Attribute
    {
        public DynamicallyCreatedAssemblyAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PreventHotReload : Attribute
    {
        
    }
    
    public interface IAssemblyChangesLoader
    {
        void DynamicallyUpdateMethodsForCreatedAssembly(Assembly dynamicallyLoadedAssemblyWithUpdates, AssemblyChangesLoaderEditorOptionsNeededInBuild editorOptions);
    }
    
    [Serializable]
    public class AssemblyChangesLoaderEditorOptionsNeededInBuild
    {
        public bool IsDidFieldsOrPropertyCountChangedCheckDisabled;
        public bool EnableExperimentalAddedFieldsSupport;

        public AssemblyChangesLoaderEditorOptionsNeededInBuild(bool isDidFieldsOrPropertyCountChangedCheckDisabled, bool enableExperimentalAddedFieldsSupport)
        {
            this.IsDidFieldsOrPropertyCountChangedCheckDisabled = isDidFieldsOrPropertyCountChangedCheckDisabled;
            this.EnableExperimentalAddedFieldsSupport = enableExperimentalAddedFieldsSupport;
        }
        
#pragma warning disable 0618
        [Obsolete("Needed for network serialization")]
#pragma warning restore 0618
        public AssemblyChangesLoaderEditorOptionsNeededInBuild()
        {
        }

        //WARN: make sure it has same params as ctor
        public void UpdateValues(bool isDidFieldsOrPropertyCountChangedCheckDisabled, bool enableExperimentalAddedFieldsSupport)
        {
            this.IsDidFieldsOrPropertyCountChangedCheckDisabled = isDidFieldsOrPropertyCountChangedCheckDisabled;
            this.EnableExperimentalAddedFieldsSupport = enableExperimentalAddedFieldsSupport;
        }
    }
}
#endif