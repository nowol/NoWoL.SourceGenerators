using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoWoL.SourceGenerators.Comparers
{
    [ExcludeFromCodeCoverage]
    public class MethodDeclarationSyntaxIsEquivalentToComparer
        : IEqualityComparer<(MethodDeclarationSyntax Node, Compilation compilation)>
    {
        public bool Equals((MethodDeclarationSyntax Node, Compilation compilation) x,
                           (MethodDeclarationSyntax Node, Compilation compilation) y)
        {
            return x.Node.IsEquivalentTo(y.Node);
        }

        public int GetHashCode((MethodDeclarationSyntax Node, Compilation compilation) obj)
        {
            return obj.Node.Identifier.Text.GetHashCode();
        }
    }
}