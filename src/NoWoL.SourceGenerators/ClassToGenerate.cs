using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace NoWoL.SourceGenerators
{
    public readonly struct ClassToGenerate
    {
        public readonly ClassDeclarationSyntax ClassDeclarationSyntax;
        public readonly INamedTypeSymbol ClassSymbol;
        public readonly AttributeData ExceptionAttribute;
        public readonly string? NameSpace;

        public ClassToGenerate(ClassDeclarationSyntax classDeclarationSyntax, INamedTypeSymbol classSymbol, AttributeData exceptionAttribute, string? nameSpace)
        {
            ClassDeclarationSyntax = classDeclarationSyntax;
            ClassSymbol = classSymbol;
            ExceptionAttribute = exceptionAttribute;
            NameSpace = nameSpace;
        }
    }
}