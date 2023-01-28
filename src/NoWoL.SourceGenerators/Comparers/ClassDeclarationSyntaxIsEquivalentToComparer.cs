using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoWoL.SourceGenerators.Comparers
{
    [ExcludeFromCodeCoverage]
    public class ClassDeclarationSyntaxIsEquivalentToComparer
        : IEqualityComparer<(ClassDeclarationSyntax Node, Compilation compilation)>
    {
        public bool Equals((ClassDeclarationSyntax Node, Compilation compilation) x,
                           (ClassDeclarationSyntax Node, Compilation compilation) y)
        {
            return x.Node.IsEquivalentTo(y.Node);
        }

        public int GetHashCode((ClassDeclarationSyntax Node, Compilation compilation) obj)
        {
            return obj.Node.Identifier.Text.GetHashCode();
        }
    }
}