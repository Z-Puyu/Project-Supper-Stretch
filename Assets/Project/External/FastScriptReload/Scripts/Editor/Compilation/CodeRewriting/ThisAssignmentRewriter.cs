﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Project.External.FastScriptReload.Scripts.Editor.Compilation.CodeRewriting
{
    class ThisAssignmentRewriter: ThisRewriterBase {
        public ThisAssignmentRewriter(bool writeRewriteReasonAsComment, bool visitIntoStructuredTrivia = false) 
            : base(writeRewriteReasonAsComment, visitIntoStructuredTrivia)
        {
        }
        
        public override SyntaxNode VisitThisExpression(ThisExpressionSyntax node)
        {
            if (node.Parent is AssignmentExpressionSyntax) {
                return this.CreateCastedThisExpression(node);
            }

            return base.VisitThisExpression(node);
        }
    }
}