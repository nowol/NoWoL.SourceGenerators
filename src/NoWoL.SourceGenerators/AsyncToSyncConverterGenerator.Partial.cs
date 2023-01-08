using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace NoWoL.SourceGenerators
{
    public partial class AsyncToSyncConverterGenerator
    {

        private static bool IsTaskOrValueTask(ISymbol? symbol)
        {
            return symbol != null
                   && symbol.ContainingNamespace != null
                   && symbol.ContainingNamespace.ToString() == "System.Threading.Tasks"
                   && (symbol.Name == nameof(System.Threading.Tasks.Task) || symbol.Name == nameof(System.Threading.Tasks.ValueTask));
        }

        public static IdentifierNameSyntax RemoveAsyncFrom(IdentifierNameSyntax ins)
        {
            return ins.WithIdentifier(SyntaxFactory.Identifier(GenerationHelpers.RemoveLastWord(ins.Identifier!.ValueText!,
                                                                                                "Async")))
                      .WithTriviaFrom(ins);
        }

        private static SyntaxToken RemoveAsyncFrom(SyntaxToken token)
        {
            var value = GenerationHelpers.RemoveLastWord(token.ValueText, "Async");

            return SyntaxFactory.Identifier(value);
        }

        private static string GetReturnsTypeDetails(ITypeSymbol returnTypeSymbol, Compilation compilation)
        {
            var nonGenericTaskClass = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task"); // todo typeof(Task).Fullname!
            if (returnTypeSymbol.Equals(nonGenericTaskClass, SymbolEqualityComparer.Default) == true)
            {
                return "void";
            }

            var nonGenericValueTaskClass = compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask");
            if (returnTypeSymbol.Equals(nonGenericValueTaskClass, SymbolEqualityComparer.Default) == true)
            {
                return "void";
            }

            if (returnTypeSymbol is INamedTypeSymbol genericType)
            {
                // Task<>
                var genericTaskClass = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
                if (genericType.OriginalDefinition.Equals(genericTaskClass, SymbolEqualityComparer.Default))
                {
                    return genericType.TypeArguments.First().ToString();
                }

                // ValueTask<>
                var genericValueTaskClass = compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1");
                if (genericType.OriginalDefinition.Equals(genericValueTaskClass, SymbolEqualityComparer.Default))
                {
                    return genericType.TypeArguments.First().ToString();
                }
            }

            return String.Empty;
        }

        private static readonly string CodeGenAttribute = GetCodeGenAttribute();

        private static string GetCodeGenAttribute()
        {
            return $@"[System.CodeDom.Compiler.GeneratedCodeAttribute(""{nameof(AsyncToSyncConverterGenerator)}"", ""{typeof(AsyncToSyncConverterGenerator).Assembly.GetName().Version}"")]";
        }

        private static void ProcesAttributeList(StringBuilder sb, SemanticModel sm, SyntaxNode n)
        {
            sb.AddLeadingTrivia(n);

            var attList = n as AttributeListSyntax;

            foreach (var attribute in attList!.Attributes)
            {
                var isGenAttr = false;
                var attSymbol = sm.GetSymbolInfo(attribute);

                if (attSymbol.Symbol is IMethodSymbol ms)
                {
                    if (ms.ContainingType.ToDisplayString() == AsyncToSyncConverterGenerator.AsyncToSyncConverterAttributeFqn)
                    {
                        isGenAttr = true;
                    }
                }

                if (isGenAttr)
                {
                    sb.AddLeadingTrivia(attribute);

                    sb.Append(CodeGenAttribute);

                    sb.AddTrailingTrivia(attribute);
                }
                else
                {
                    sb.Append(attList!.OpenBracketToken.ToString());

                    sb.Append(attribute.ToFullString());

                    sb.Append(attList!.CloseBracketToken.ToString());
                }
            }

            sb.AddTrailingTrivia(n);
        }

        private static bool IsCancellationTokenRelated(Compilation compilation, SemanticModel sm, SyntaxNode node)
        {
            if (node.IsKind(SyntaxKind.ExpressionStatement)
                && node is ExpressionStatementSyntax exp)
            {
                if (exp!.Expression is InvocationExpressionSyntax ies)
                {
                    if (ies.Expression is MemberAccessExpressionSyntax maes)
                    {
                        var ti = sm.GetTypeInfo(maes.Expression);

                        if (ti.ConvertedType != null
                            && ti.ConvertedType!.Equals(compilation.GetTypeByMetadataName("System.Threading.CancellationToken"),
                                                        SymbolEqualityComparer.Default))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            if (node.IsKind(SyntaxKind.Argument)
                && node is ArgumentSyntax argSyntax)
            {
                var ti = sm.GetTypeInfo(argSyntax.Expression);
                if (ti.ConvertedType != null
                    && ti.ConvertedType!.Equals(compilation.GetTypeByMetadataName("System.Threading.CancellationToken"),
                                                SymbolEqualityComparer.Default))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static class PotatoExtension
    {
        public static void AppendWithTrivia(this StringBuilder sb, SyntaxNode? n)
        {
            if (n != null)
            {
                sb.Append(n.ToFullString());
            }
        }

        public static void AddLeadingTrivia(this StringBuilder sb, SyntaxNode n)
        {
            if (n!.HasLeadingTrivia)
            {
                var triviaList = n.GetLeadingTrivia();

                foreach (var trivia in triviaList)
                {
                    if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    {
                        var structure = trivia.GetStructure();

                        if (structure != null)
                        {
                            foreach (var childNodesAndToken in structure.ChildNodesAndTokens())
                            {
                                if (childNodesAndToken.IsToken)
                                {
                                    sb.Append(childNodesAndToken.ToFullString());
                                }
                                else
                                {
                                    var node = childNodesAndToken.AsNode()!;
                                    bool ignoreLine = false;

                                    if (node is XmlElementSyntax xes)
                                    {
                                        if (xes.StartTag.Name.ToString() == "param")
                                        {
                                            if (xes.StartTag.Attributes.FirstOrDefault(x => x is XmlNameAttributeSyntax) is XmlNameAttributeSyntax nameAttr)
                                            {
                                                if (nameAttr.Identifier.Identifier.ValueText == "cancellationToken")
                                                {
                                                    ignoreLine = true;
                                                }
                                                else if (nameAttr.Identifier.Identifier.ValueText.EndsWith("Async") == true)
                                                {
                                                    var newNameAttr = nameAttr.WithIdentifier(AsyncToSyncConverterGenerator.RemoveAsyncFrom(nameAttr.Identifier));
                                                    var newStartTag = xes.StartTag.WithAttributes(xes.StartTag.Attributes.Replace(nameAttr, newNameAttr));
                                                    var newXes = xes.WithStartTag(newStartTag);

                                                    sb.AppendWithTrivia(newXes);
                                                    ignoreLine = true;
                                                }
                                            }
                                        }
                                    }

                                    if (!ignoreLine)
                                    {
                                        sb.Append(node.ToFullString());
                                    }
                                }
                            }
                        }
                        else
                        {
                            sb.Append(trivia.ToFullString());
                        }
                    }
                    else if (trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                    { }
                    else
                    {
                        sb.Append(trivia.ToFullString());
                    }
                }
            }
        }

        public static void AddTrailingTrivia(this StringBuilder sb, SyntaxNode n)
        {
            if (n!.HasTrailingTrivia)
            {
                sb.Append(n.GetTrailingTrivia().ToFullString());
            }
        }

        public static void AddLeadingTrivia(this StringBuilder sb, SyntaxToken token)
        {
            if (token!.HasLeadingTrivia)
            {
                sb.Append(token.LeadingTrivia.ToFullString());
            }
        }

        public static void AddTrailingTrivia(this StringBuilder sb, SyntaxToken token)
        {
            if (token!.HasTrailingTrivia)
            {
                sb.Append(token.TrailingTrivia.ToFullString());
            }
        }
    }
}
