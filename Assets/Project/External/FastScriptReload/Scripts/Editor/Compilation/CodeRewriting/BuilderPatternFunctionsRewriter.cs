using System.Linq;
using ImmersiveVrToolsCommon.Runtime.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Project.External.FastScriptReload.Scripts.Runtime;

namespace Project.External.FastScriptReload.Scripts.Editor.Compilation.CodeRewriting
{
    class BuilderPatternFunctionsRewriter : FastScriptReloadCodeRewriterBase
    {
        public BuilderPatternFunctionsRewriter(bool writeRewriteReasonAsComment, bool visitIntoStructuredTrivia = false) 
            : base(writeRewriteReasonAsComment, visitIntoStructuredTrivia)
        {
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var ancestorName = node.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText;
            if (string.IsNullOrEmpty(ancestorName)) //TODO: that happens too often for some methods, odd. Why? At very least don't spam user
            {
                LoggerScoped.LogWarning($"Unable to find ancestor for node '{node.ToFullString()}'");
            }
            else
            {
                var ancestorNameWithoutPatchedPostfix = ancestorName.Replace(AssemblyChangesLoader.ClassnamePatchedPostfix, "");

                if (node.ReturnType is IdentifierNameSyntax name && name.Identifier.ValueText == ancestorNameWithoutPatchedPostfix)
                {
                    return node.WithReturnType(SyntaxFactory.IdentifierName(ancestorName + " "));
                }
            }
            
            return base.VisitMethodDeclaration(node);
        }
    }
}