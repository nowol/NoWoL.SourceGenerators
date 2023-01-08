using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoWoL.SourceGenerators
{
    internal partial class AsyncToSyncProcessor // Utilities
    {
        private static readonly string CodeGenAttribute = GetCodeGenAttribute();

        private static string GetCodeGenAttribute()
        {
            return $@"[System.CodeDom.Compiler.GeneratedCodeAttribute(""{nameof(AsyncToSyncConverterGenerator)}"", ""{typeof(AsyncToSyncConverterGenerator).Assembly.GetName().Version}"")]";
        }

        private SyntaxNode UnfoldAwaitExpression(SyntaxNode node)
        {
            if (node.IsKind(SyntaxKind.AwaitExpression)
                && node is AwaitExpressionSyntax awaitSyntax)
            {
                if (TryConfigureAwaitRelated(awaitSyntax.Expression, out var maes))
                {
                    node = maes!.Expression;
                }
                else
                {
                    node = awaitSyntax.Expression;
                }
            }

            return node;
        }
        private bool IsTaskOrValueTask(ISymbol? symbol)
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

        private SyntaxToken RemoveAsyncFrom(SyntaxToken token)
        {
            var value = GenerationHelpers.RemoveLastWord(token.ValueText, "Async");

            return SyntaxFactory.Identifier(value);
        }

        private string GetReturnsTypeDetails(ITypeSymbol returnTypeSymbol, Compilation compilation)
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

        private bool IsCancellationTokenRelated(SyntaxNode node)
        {
            SyntaxNode? snToAnalyze = null;

            if (node.IsKind(SyntaxKind.ExpressionStatement)
                && node is ExpressionStatementSyntax exp)
            {
                if (exp!.Expression is InvocationExpressionSyntax ies)
                {
                    if (ies.Expression is MemberAccessExpressionSyntax maes)
                    {
                        snToAnalyze = maes.Expression;
                    }
                }
            }

            if (node.IsKind(SyntaxKind.Argument)
                && node is ArgumentSyntax argSyntax)
            {
                snToAnalyze = argSyntax.Expression;
            }

            if (snToAnalyze != null)
            {
                var ti = _analysisContext.SemanticModel.GetTypeInfo(snToAnalyze);
                if (ti.ConvertedType != null
                    && ti.ConvertedType!.Equals(_analysisContext.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken"),
                                                SymbolEqualityComparer.Default))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryConfigureAwaitRelated(SyntaxNode node, out MemberAccessExpressionSyntax? maes)
        {
            maes = null;

            if (node.IsKind(SyntaxKind.InvocationExpression)
                && node is InvocationExpressionSyntax ies)
            {
                if (ies!.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    maes = memberAccess;
                }
            }

            if (maes != null
                &&
                (
                    string.Equals(maes.Name.Identifier.ValueText,
                                  "ConfigureAwait",
                                  StringComparison.Ordinal)
                    || string.Equals(maes.Name.Identifier.ValueText,
                                     "ConfigureAwaitWithCulture",
                                     StringComparison.Ordinal)
                )
                && maes.Expression is InvocationExpressionSyntax inv)
            {
                if (inv.Expression is IdentifierNameSyntax identifierNameSyntax)
                {
                    return true;
                }

                return true;
            }

            maes = null;
            return false;
        }

        private TypeArgumentListSyntax RemoveCancellationToken(TypeArgumentListSyntax argList)
        {
            var newArgList = argList;

            if (argList.Arguments.Count > 0)
            {
                int cpt = argList.Arguments.Count - 1;

                while (cpt >= 0)
                {
                    var arg = argList.Arguments[cpt];

                    var ti = _analysisContext.SemanticModel.GetTypeInfo(arg);

                    if (ti.ConvertedType != null
                        && ti.ConvertedType!.Equals(_analysisContext.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken"),
                                                    SymbolEqualityComparer.Default))
                    {
                        newArgList = newArgList.WithArguments(newArgList.Arguments.RemoveAt(cpt));
                    }

                    cpt--;
                }
            }

            return newArgList;
        }

        private ArgumentListSyntax RemoveCancellationToken(ArgumentListSyntax argList)
        {
            var newArgList = argList;

            if (argList.Arguments.Count > 0)
            {
                int cpt = argList.Arguments.Count - 1;

                while (cpt >= 0)
                {
                    var arg = argList.Arguments[cpt];

                    var ti = _analysisContext.SemanticModel.GetTypeInfo(arg.Expression);

                    if (ti.ConvertedType != null
                        && ti.ConvertedType!.Equals(_analysisContext.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken"),
                                                    SymbolEqualityComparer.Default))
                    {
                        newArgList = newArgList.WithArguments(newArgList.Arguments.RemoveAt(cpt));
                    }

                    cpt--;
                }
            }

            return newArgList;
        }

        private bool TryTaskDelay(SyntaxNode syntaxNode, out ArgumentListSyntax? argList)
        {
            Debug.Assert(syntaxNode is not AwaitExpressionSyntax);

            InvocationExpressionSyntax ? snToAnalyze = null;

            if (syntaxNode.IsKind(SyntaxKind.InvocationExpression)
                     && syntaxNode is InvocationExpressionSyntax ies)
            {
                snToAnalyze = ies;
            }

            if (snToAnalyze != null)
            {
                var symbol = _analysisContext.SemanticModel.GetSymbolInfo(snToAnalyze).Symbol;
                if (symbol != null
                    && symbol.ContainingType != null
                    && symbol.ContainingType.ToString() == "System.Threading.Tasks.Task"
                    && symbol.Name == nameof(System.Threading.Tasks.Task.Delay))
                {
                    argList = RemoveCancellationToken(snToAnalyze.ArgumentList).WithTriviaFrom(snToAnalyze.ArgumentList);

                    return true;
                }
            }

            argList = null;

            return false;
        }
    }
}
