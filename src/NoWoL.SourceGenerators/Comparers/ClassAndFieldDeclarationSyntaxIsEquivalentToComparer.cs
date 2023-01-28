using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoWoL.SourceGenerators.Comparers
{
    [ExcludeFromCodeCoverage]
    public class ClassAndFieldDeclarationSyntaxIsEquivalentToComparer
        : IEqualityComparer<(ClassDeclarationSyntax ClassNode, FieldDeclarationSyntax FieldNode)>
    {
        public bool Equals((ClassDeclarationSyntax ClassNode, FieldDeclarationSyntax FieldNode) x,
                           (ClassDeclarationSyntax ClassNode, FieldDeclarationSyntax FieldNode) y)
        {
            if (!String.Equals(x.ClassNode.Identifier.Text,
                               y.ClassNode.Identifier.Text))
            {
                return false;
            }

            if (x.FieldNode.Declaration.Variables.Count != y.FieldNode.Declaration.Variables.Count)
            {
                return false;
            }

            for (var i = 0; i < x.FieldNode.Declaration.Variables.Count; i++)
            {
                var v = x.FieldNode.Declaration.Variables[i];
                var vo = y.FieldNode.Declaration.Variables[i];

                if (v.Identifier.Text != vo.Identifier.Text)
                {
                    return false;
                }
            }

            return true;

            //return x.ClassNode.IsEquivalentTo(y.ClassNode)
            //       && x.FieldNode.IsEquivalentTo(y.FieldNode);
        }

        public int GetHashCode((ClassDeclarationSyntax ClassNode, FieldDeclarationSyntax FieldNode) obj)
        {
            HashCode hc = default;

            hc.Add(obj.ClassNode.Identifier.Text);

            foreach (var variable in obj.FieldNode.Declaration.Variables)
            {
                hc.Add(variable.Identifier.Text);
            }
            
            return hc.ToHashCode();
        }
    }
}