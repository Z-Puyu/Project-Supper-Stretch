using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Project.External.FastScriptReload.Scripts.Editor.Compilation.CodeRewriting
{
    public class NewFieldDeclaration
    {
        public string FieldName { get; }
        public string TypeName { get; }
        public FieldDeclarationSyntax FieldDeclarationSyntax { get; } //TODO: PERF: will that block whole tree from being garbage collected

        public NewFieldDeclaration(string fieldName, string typeName, FieldDeclarationSyntax fieldDeclarationSyntax)
        {
            this.FieldName = fieldName;
            this.TypeName = typeName;
            this.FieldDeclarationSyntax = fieldDeclarationSyntax;
        }
    }
}