using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NoWoL.SourceGenerators
{
    public readonly struct AlwaysInitializedPropertyClassToGenerate
    {
        public readonly ClassDeclarationSyntax ClassDeclarationSyntax;
        public readonly ImmutableArray<FieldDeclarationSyntax> Fields;
        public readonly string? NameSpace;

        public AlwaysInitializedPropertyClassToGenerate(ClassDeclarationSyntax classDeclarationSyntax, ImmutableArray<FieldDeclarationSyntax> fields, string? nameSpace)
        {
            ClassDeclarationSyntax = classDeclarationSyntax;
            Fields = fields;
            NameSpace = nameSpace;
        }
    }
}