using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace NoWoL.SourceGenerators
{
    public readonly struct ExceptionClassToGenerate
    {
        public readonly ClassDeclarationSyntax ClassDeclarationSyntax;
        public readonly List<AttributeData> ExceptionAttributes;
        public readonly string? NameSpace;

        public ExceptionClassToGenerate(ClassDeclarationSyntax classDeclarationSyntax, List<AttributeData> exceptionAttributes, string? nameSpace)
        {
            ClassDeclarationSyntax = classDeclarationSyntax;
            ExceptionAttributes = exceptionAttributes;
            NameSpace = nameSpace;
        }
    }
}