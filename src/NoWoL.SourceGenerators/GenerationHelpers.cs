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
        public static bool IsPartialClass(ClassDeclarationSyntax target)
        {
            return target.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
        }

        public static string? GetNamespace(ClassDeclarationSyntax target)
        {
            if (target.AncestorsAndSelf().FirstOrDefault(x => x.IsKind(SyntaxKind.NamespaceDeclaration)) is NamespaceDeclarationSyntax ns)
            {
                return ns.Name.ToString();
            }

            if (target.AncestorsAndSelf().FirstOrDefault(x => x.IsKind(SyntaxKind.FileScopedNamespaceDeclaration)) is FileScopedNamespaceDeclarationSyntax scopedNs)
            {
                return scopedNs.Name.ToString();
            }

            return null;
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
