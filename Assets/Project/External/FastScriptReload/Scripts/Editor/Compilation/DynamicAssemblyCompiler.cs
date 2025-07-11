﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using ImmersiveVRTools.Runtime.Common;
using ImmersiveVrToolsCommon.Runtime.Logging;

namespace Project.External.FastScriptReload.Scripts.Editor.Compilation
{
    public class DynamicAssemblyCompiler
    {
        public static CompileResult Compile(List<string> filePathsWithSourceCode, UnityMainThreadDispatcher unityMainThreadDispatcher)
        {
            var sw = new Stopwatch();
            sw.Start();
        
#if FastScriptReload_CompileViaMCS
        var result = McsExeDynamicCompilation.Compile(filePathsWithSourceCode);
#else
            var compileResult = DotnetExeDynamicCompilation.Compile(filePathsWithSourceCode, unityMainThreadDispatcher);
#endif  
        
            LoggerScoped.Log($"Files: {string.Join(",", filePathsWithSourceCode.Select(fn => new FileInfo(fn).Name))} changed " +
#if UNITY_2021_1_OR_NEWER
                             $"<a href=\"{compileResult.SourceCodeCombinedFileLocation}\" line=\"1\">(click here to debug [in bottom details pane])</a>" +
#else
                            "(to debug go to Fast Script Reload -> Start Screen -> Debugging -> Auto-open generated source file for debugging)" +
#endif
                             $" - compilation (took {sw.ElapsedMilliseconds}ms)"
#if ImmersiveVrTools_DebugEnabled
                            + $" (of which creating internal visible to assemblies {compileResult.CreatingAssemblyWithInternalsVisibleToTook})ms"
#endif
);
            
            return compileResult;
        }
    }

    public class CompileResult
    {
        public Assembly CompiledAssembly { get; }
        public string CompiledAssemblyPath { get; }
        public List<string> MessagesFromCompilerProcess { get; }
        public bool IsError => string.IsNullOrEmpty(this.CompiledAssemblyPath);
        public int NativeCompilerReturnValue { get; }
        public string SourceCodeCombined { get; }
        public string SourceCodeCombinedFileLocation { get; }
        public float CreatingAssemblyWithInternalsVisibleToTook { get; }

        public CompileResult(string compiledAssemblyPath, List<string> messagesFromCompilerProcess, int nativeCompilerReturnValue, Assembly compiledAssembly, string sourceCodeCombined, string sourceCodeCombinedFileLocation, float creatingAssemblyWithInternalsVisibleToTook)
        {
            this.CompiledAssemblyPath = compiledAssemblyPath;
            this.MessagesFromCompilerProcess = messagesFromCompilerProcess;
            this.NativeCompilerReturnValue = nativeCompilerReturnValue;
            this.CompiledAssembly = compiledAssembly;
            this.SourceCodeCombined = sourceCodeCombined;
            this.SourceCodeCombinedFileLocation = sourceCodeCombinedFileLocation;
            this.CreatingAssemblyWithInternalsVisibleToTook = creatingAssemblyWithInternalsVisibleToTook;
        }
    }
}