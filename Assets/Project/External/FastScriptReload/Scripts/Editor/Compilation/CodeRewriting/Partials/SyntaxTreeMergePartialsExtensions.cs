using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Project.External.FastScriptReload.Scripts.Editor.Compilation.CodeRewriting.Partials
{
    public static class SyntaxTreeMergePartialsExtensions
    {
        public static IEnumerable<SyntaxTree> MergePartials(
                this IEnumerable<SyntaxTree> trees,
                IEnumerable<string> definedPreprocessorSymbols)
        {
            return trees
                    .Select(tree => PartialTreeProcessor.ProcessPartialTree(tree, definedPreprocessorSymbols))
                    .ToList();
        }
    }
}
