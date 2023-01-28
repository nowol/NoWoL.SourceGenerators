using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoWoL.SourceGenerators.Comparers
{
    [ExcludeFromCodeCoverage]
    public class ClassDeclarationSyntaxWithFieldsIsEquivalentToComparer
        : IEqualityComparer<(ClassDeclarationSyntax Node, ImmutableArray<FieldDeclarationSyntax> Fields)>
    {
        public bool Equals((ClassDeclarationSyntax Node, ImmutableArray<FieldDeclarationSyntax> Fields) x,
                           (ClassDeclarationSyntax Node, ImmutableArray<FieldDeclarationSyntax> Fields) y)
        {
            if (!SameClass(x.Node, y.Node))
            {
                Log(x, y, "1");
                return false;
            }
            
            //if (!x.Node.IsEquivalentTo(y.Node))
            //{
            //    Log(x, y, "1");
            //    return false;
            //}

            //return x.Fields.Equals(y.Fields);

            if (x.Fields.Length != y.Fields.Length)
            {
                Log(x, y, "2");
                return false;
            }

            for (var i = 0; i < x.Fields.Length; i++)
            {
                var field = x.Fields[i];
                var otherField = y.Fields[i];

                if (!field.IsEquivalentTo(otherField))
                {
                    Log(x, y, "3");
                    return false;
                }
            }

            return true;
        }

        private bool SameClass(ClassDeclarationSyntax xNode, ClassDeclarationSyntax yNode)
        {
            if (!String.Equals(xNode.Identifier.Text,
                               yNode.Identifier.Text))
            {
                return false;
            }

            if (!CompareTokenList(xNode.Modifiers,
                                  yNode.Modifiers))
            {
                return false;
            }

            if (!CompareSyntaxList(xNode.AttributeLists,
                                   yNode.AttributeLists))
            {
                return false;
            }

            return true;
        }

        private bool CompareSyntaxList<T>(SyntaxList<T> xList, SyntaxList<T> yList)
            where T : SyntaxNode
        {
            if (xList.Count != yList.Count)
            {
                return false;
            }

            for (var i = 0; i < xList.Count; i++)
            {
                var xMod = xList[0];
                var yMod = yList[0];

                if (!xMod.IsEquivalentTo(yMod))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CompareTokenList(SyntaxTokenList xList, SyntaxTokenList yList)
        {
            if (xList.Count != yList.Count)
            {
                return false;
            }

            for (var i = 0; i < xList.Count; i++)
            {
                var xMod = xList[0];
                var yMod = yList[0];

                if (!xMod.IsEquivalentTo(yMod))
                {
                    return false;
                }
            }

            return true;
        }

        private void Log((ClassDeclarationSyntax Node, ImmutableArray<FieldDeclarationSyntax> Fields) valueTuple, (ClassDeclarationSyntax Node, ImmutableArray<FieldDeclarationSyntax> Fields) valueTuple1
                         , string id)
        {
            var s = new StringBuilder();

            s.AppendLine($"[nwl]==== {id} ====");
            s.Append(valueTuple.Node.Identifier.Text);

            foreach (var fieldDeclarationSyntax in valueTuple.Fields)
            {
                s.Append("---" + fieldDeclarationSyntax.Declaration.Variables.First().Identifier.Text);
            }

            s.Append(valueTuple1.Node.Identifier.Text);

            foreach (var fieldDeclarationSyntax in valueTuple1.Fields)
            {
                s.Append("---" + fieldDeclarationSyntax.Declaration.Variables.First().Identifier.Text);
            }

            s.AppendLine("/========");
            s.AppendLine(valueTuple.Node.ToFullString());
            s.AppendLine(valueTuple1.Node.ToFullString());


            System.Diagnostics.Trace.WriteLine(s.ToString());
        }

        public int GetHashCode((ClassDeclarationSyntax Node, ImmutableArray<FieldDeclarationSyntax> Fields) obj)
        {
            HashCode hashCode = default;

            hashCode.Add(obj.Node.Identifier.Text);

            foreach (var field in obj.Fields)
            {
                foreach (var variable in field.Declaration.Variables)
                {
                    hashCode.Add(variable.Identifier.Text);
                }
            }

            return hashCode.ToHashCode();
        }
    }
}