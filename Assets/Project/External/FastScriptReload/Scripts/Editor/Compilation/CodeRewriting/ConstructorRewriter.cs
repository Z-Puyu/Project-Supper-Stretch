using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Project.External.FastScriptReload.Scripts.Runtime;

namespace Project.External.FastScriptReload.Scripts.Editor.Compilation.CodeRewriting
{
	class ConstructorRewriter : FastScriptReloadCodeRewriterBase
        {
	        private readonly bool _adjustCtorOnlyForNonNestedTypes;
	        
	        public ConstructorRewriter(bool adjustCtorOnlyForNonNestedTypes, bool writeRewriteReasonAsComment)
				: base(writeRewriteReasonAsComment)
	        {
		        this._adjustCtorOnlyForNonNestedTypes = adjustCtorOnlyForNonNestedTypes;
	        }
	        
	        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
	        {
		        if (this._adjustCtorOnlyForNonNestedTypes)
		        {
			        var typeNestedLevel = node.Ancestors().Count(a => a is TypeDeclarationSyntax);
			        if (typeNestedLevel == 1)
			        {
				        return this.AdjustCtorOrDestructorNameForTypeAdjustment(node, node.Identifier);
			        }
		        }
		        else
		        {
			        return this.AdjustCtorOrDestructorNameForTypeAdjustment(node, node.Identifier);
		        }

		        return base.VisitConstructorDeclaration(node);
	        }

	        public override SyntaxNode VisitDestructorDeclaration(DestructorDeclarationSyntax node)
	        {
		        if (this._adjustCtorOnlyForNonNestedTypes)
		        {
			        var typeNestedLevel = node.Ancestors().Count(a => a is TypeDeclarationSyntax);
			        if (typeNestedLevel == 1)
			        {
				        return this.AdjustCtorOrDestructorNameForTypeAdjustment(node, node.Identifier);
			        }
		        }
		        else
		        {
			        return this.AdjustCtorOrDestructorNameForTypeAdjustment(node, node.Identifier);
		        }
		        
		        return base.VisitDestructorDeclaration(node);
	        }

	        private SyntaxNode AdjustCtorOrDestructorNameForTypeAdjustment(BaseMethodDeclarationSyntax node, SyntaxToken nodeIdentifier)
	        {
		        var typeName = (node.Ancestors().First(n => n is TypeDeclarationSyntax) as TypeDeclarationSyntax).Identifier.ToString();
		        if (!nodeIdentifier.ToFullString().Contains(typeName))
		        {
			        //Used Roslyn version bug, some static methods are also interpreted as ctors, eg
			        // public static void Method()
			        // {
			        //    Bar(); //treated as Ctor declaration...
			        // }
			        //
			        // private static void Bar() 
			        // {
			        //  
			        // }
			        return node;
		        }
		        
		        if (!typeName.EndsWith(AssemblyChangesLoader.ClassnamePatchedPostfix))
		        {
			        typeName += AssemblyChangesLoader.ClassnamePatchedPostfix;
		        }

		        return this.AddRewriteCommentIfNeeded(
			        node.ReplaceToken(nodeIdentifier, SyntaxFactory.Identifier(typeName)), 
			        $"{nameof(ConstructorRewriter)}:{nameof(this.AdjustCtorOrDestructorNameForTypeAdjustment)}"
			    );
	        }
        }
}