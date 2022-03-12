using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoWoL.SourceGenerators
{
    internal static class GenerationHelpers
    {
        public static bool IsPartialClass(ClassDeclarationSyntax syntax)
        {
            return syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
        }

        public static string GetNamespace(ClassDeclarationSyntax syntax)
        {
            // determine the namespace the class is declared in, if any
            string nameSpace = string.Empty;
            SyntaxNode? potentialNamespaceParent = syntax.Parent;
            while (potentialNamespaceParent != null &&
                   potentialNamespaceParent is not NamespaceDeclarationSyntax
                   && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
            {
                nameSpace = namespaceParent.Name.ToString();
                while (true)
                {
                    if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    {
                        break;
                    }

                    namespaceParent = parent;
                    nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                }
            }

            return nameSpace;
        }

        private static readonly MD5CryptoServiceProvider Md5Algo = new ();
        public static string Md5(string value)
        {
            lock (Md5Algo)
            {
                var hashBytes = Md5Algo.ComputeHash(Encoding.UTF8.GetBytes(value));

                return HexStringFromBytes(hashBytes);
            }
        }

        private static string HexStringFromBytes(IEnumerable<byte> bytes)
        {
            var sb = new StringBuilder();

            foreach (var b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString();
        }
    }
}
