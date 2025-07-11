﻿using System.IO;
using System.Reflection;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;

namespace Project.External.FastScriptReload.Scripts.Editor.AssemblyPostProcess
{
    [InitializeOnLoad]
    public static class AddInternalsVisibleToForAllUserAssembliesPostProcess 
    {
        public static readonly DirectoryInfo AdjustedAssemblyRoot;

        static AddInternalsVisibleToForAllUserAssembliesPostProcess()
        {
            AddInternalsVisibleToForAllUserAssembliesPostProcess.AdjustedAssemblyRoot = new DirectoryInfo(Path.Combine(Application.dataPath, "..", "Temp", "Fast Script Reload", "AdjustedDlls"));
        }
        
        public static string CreateAssemblyWithInternalsContentsVisibleTo(Assembly changedAssembly, string visibleToAssemblyName)
        {
            if(!AddInternalsVisibleToForAllUserAssembliesPostProcess.AdjustedAssemblyRoot.Exists)
                    AddInternalsVisibleToForAllUserAssembliesPostProcess.AdjustedAssemblyRoot.Create();

            using (var assembly = AssemblyDefinition.ReadAssembly(changedAssembly.Location, new ReaderParameters { ReadWrite = false }))
            {
                var mainModule = assembly.MainModule;

                var attributeCtor = mainModule.ImportReference(
                    typeof(System.Runtime.CompilerServices.InternalsVisibleToAttribute).GetConstructor(new[] { typeof(string) })
                );

                var attribute = new CustomAttribute(attributeCtor);
                attribute.ConstructorArguments.Add(
                    new CustomAttributeArgument(mainModule.TypeSystem.String, visibleToAssemblyName)
                );

                assembly.CustomAttributes.Add(attribute);
                
                var newAssemblyPath = new FileInfo(Path.Combine(AddInternalsVisibleToForAllUserAssembliesPostProcess.AdjustedAssemblyRoot.FullName, assembly.Name.Name) + ".dll").FullName;
                assembly.Write(newAssemblyPath);

                return newAssemblyPath;
            }
        }
    }
}