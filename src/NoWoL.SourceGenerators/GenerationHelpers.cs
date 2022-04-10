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
        public static bool IsPartialClass(ClassDeclarationSyntax? syntax)
        {
            if (syntax == null)
            {
                return false;
            }

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

        public static string GetModifier(ClassDeclarationSyntax target, SyntaxKind kind, bool addTrailingSpace = false)
        {
            var modifier = target.Modifiers.FirstOrDefault(m => m.IsKind(kind)).ValueText;

            if (!addTrailingSpace || String.IsNullOrWhiteSpace(modifier))
            {
                return modifier;
            }

            return modifier + " ";
        }

        public static string BuildClassDefinition(ClassDeclarationSyntax classDef)
        {
            return BuildClassDefinition(classDef,
                                        classDef.Identifier.ValueText);
        }

        public static string BuildClassDefinition(ClassDeclarationSyntax classDef, string className)
        {
            var modifiers = GetClassAccessModifiers(classDef, addTrailingSpace: true);
            var staticDef = GenerationHelpers.GetModifier(classDef, SyntaxKind.StaticKeyword, addTrailingSpace: true);
            var partialDef = GenerationHelpers.GetModifier(classDef, SyntaxKind.PartialKeyword, addTrailingSpace: true);
            var abstractDef = GenerationHelpers.GetModifier(classDef, SyntaxKind.AbstractKeyword, addTrailingSpace: true);

            return $"{modifiers}{staticDef}{abstractDef}{partialDef}class {className}";
        }

        public static string RemoveLastWord(string value, string word)
        {
            if (value.EndsWith(word, StringComparison.OrdinalIgnoreCase))
            {
                return value.Substring(0,
                                       value.Length - word.Length);
            }

            return value;
        }

        public static string GetClassAccessModifiers(ClassDeclarationSyntax target, bool addTrailingSpace = false)
        {
            var modifiersSyntax = target.Modifiers.Where(m => m.IsKind(SyntaxKind.PrivateKeyword)
                                                              || m.IsKind(SyntaxKind.PublicKeyword)
                                                              || m.IsKind(SyntaxKind.ProtectedKeyword)
                                                              || m.IsKind(SyntaxKind.InternalKeyword));

            var modifiers = String.Join(" ",
                                        modifiersSyntax.Select(x => x.ValueText));

            if (!addTrailingSpace || String.IsNullOrWhiteSpace(modifiers))
            {
                return modifiers;
            }

            return modifiers + " ";
        }

        public static bool IdentifierEndsWithAsync(IdentifierNameSyntax ins)
        {
            return EndsWithAsync(ins.Identifier.ValueText);
        }

        public static bool EndsWithAsync(string valueText)
        {
            return valueText.EndsWith("Async");
        }

        public static string ConvertErrorCode(AsyncToSyncErrorCode errorCode)
        {
            return errorCode switch
                   {
                       AsyncToSyncErrorCode.AwaitedMethodMustEndWithAsync => "NWL0001",
                       AsyncToSyncErrorCode.ReturnedMethodMustEndWithAsync => "NWL0002",
                       AsyncToSyncErrorCode.AttributeMustBeAppliedToAClassEndingWithAsync => "NWL0003",
                       AsyncToSyncErrorCode.AttributeMustBeAppliedToPartialClass => "NWL0004",
                       AsyncToSyncErrorCode.AttributeMustBeAppliedInPartialClassHierarchy => "NWL0005",
                       AsyncToSyncErrorCode.MethodMustBeInNameSpace => "NWL0006",
                       AsyncToSyncErrorCode.MethodMustReturnTask => "NWL0007",
                       AsyncToSyncErrorCode.UnexpectedException => "NWL0008",
                       _ => throw new ArgumentOutOfRangeException(nameof(errorCode),
                                                                  errorCode,
                                                                  null)
                   };
        }

        public static string ConvertErrorCode(ExceptionGeneratorErrorCode errorCode)
        {
            return errorCode switch
                   {
                       ExceptionGeneratorErrorCode.UnexpectedException => "NWL1000",
                       ExceptionGeneratorErrorCode.MustBeInParentPartialClass => "NWL1001",
                       ExceptionGeneratorErrorCode.MethodClassMustBeInNamespace => "NWL1002",
                       ExceptionGeneratorErrorCode.MethodMustBePartial => "NWL1003",
                       _ => throw new ArgumentOutOfRangeException(nameof(errorCode),
                                                                  errorCode,
                                                                  null)
                   };
        }

        public static bool IsOperationCanceledException(Exception ex)
        {
            return ex is OperationCanceledException;
        }
    }
}
