﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Project.External.FastScriptReload.Scripts.Editor.Compilation.CodeRewriting
{
    class FieldsWalker : CSharpSyntaxWalker {
        private readonly Dictionary<string, List<NewFieldDeclaration>> _typeNameToFieldDeclarations = new Dictionary<string, List<NewFieldDeclaration>>();

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var className = node.Identifier;
            var fullClassName = RoslynUtils.GetMemberFQDN(node, className.ToString());
            if(!this._typeNameToFieldDeclarations.ContainsKey(fullClassName)) {
                this._typeNameToFieldDeclarations[fullClassName] = new List<NewFieldDeclaration>();
            }

            base.VisitClassDeclaration(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            var fieldName = node.Declaration.Variables.First().Identifier.ToString();
            var fullClassName = RoslynUtils.GetMemberFQDNWithoutMemberName(node);
            if(!this._typeNameToFieldDeclarations.ContainsKey(fullClassName)) {
                this._typeNameToFieldDeclarations[fullClassName] = new List<NewFieldDeclaration>();
            }
		
            this._typeNameToFieldDeclarations[fullClassName].Add(new NewFieldDeclaration(fieldName, node.Declaration.Type.ToString(), node));
		
            base.VisitFieldDeclaration(node);
        }

        public Dictionary<string, List<NewFieldDeclaration>> GetTypeToFieldDeclarations() {
            return this._typeNameToFieldDeclarations;
        }
        
        public List<string> GetTypeNames() {
            return this._typeNameToFieldDeclarations.Select(kv => kv.Key).ToList();
        }
    }
}