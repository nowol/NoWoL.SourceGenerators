using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace NoWoL.SourceGenerators
{
    public readonly struct ClassToGenerate
    {
        public readonly ClassDeclarationSyntax ClassDeclarationSyntax;
        public readonly INamedTypeSymbol ClassSymbol;
        public readonly List<AttributeData> ExceptionAttributes;
        public readonly string? NameSpace;

        public ClassToGenerate(ClassDeclarationSyntax classDeclarationSyntax, INamedTypeSymbol classSymbol, List<AttributeData> exceptionAttributes, string? nameSpace)
        {
            ClassDeclarationSyntax = classDeclarationSyntax;
            ClassSymbol = classSymbol;
            ExceptionAttributes = exceptionAttributes;
            NameSpace = nameSpace;
        }
    }
}