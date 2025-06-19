using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Project.External.FastScriptReload.Scripts.Runtime;

namespace Project.External.FastScriptReload.Scripts.Editor.Compilation.CodeRewriting
{
	class RecordeRewriter : FastScriptReloadCodeRewriterBase
    {
        public RecordeRewriter(bool writeRewriteReasonAsComment)
	        : base(writeRewriteReasonAsComment)
        {
	        
        }
        
        public override SyntaxNode VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
	        return this.AdjustRecordName(node, node.Identifier);
        }

        private SyntaxNode AdjustRecordName(RecordDeclarationSyntax node, SyntaxToken nodeIdentifier)
        {
	        var typeName = nodeIdentifier.ToString(); //Not ToFullString() as it may include spaces and break.
	        
	        if (!typeName.EndsWith(AssemblyChangesLoader.ClassnamePatchedPostfix))
	        {
		        typeName += AssemblyChangesLoader.ClassnamePatchedPostfix;
	        }

	        return this.AddRewriteCommentIfNeeded(
		        node.ReplaceToken(nodeIdentifier, SyntaxFactory.Identifier(typeName)), 
		        $"{nameof(RecordeRewriter)}:{nameof(this.AdjustRecordName)}"
		    );
        }
    }
}