using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoWoL.SourceGenerators
{
    internal partial class AsyncToSyncProcessor // Diagnostics
    {
        public (bool Result, TypeDeclarationSyntax? TypeDeclaration, string? NameSpace) AnalyzeAncestors(MethodDeclarationSyntax target)
        {
            if (!target.Identifier.ValueText.EndsWith("Async", StringComparison.OrdinalIgnoreCase))
            {
                _analysisContext.AddDiagnostic(AsyncToSyncErrorCode.AttributeMustBeAppliedToAClassEndingWithAsync,
                                               "The [AsyncToSyncConverter] must be applied to a method ending with Async.",
                                               target.GetLocation());
                return (false, null, null);
            }

            var typeDeclaration = target.FirstAncestorOrSelf<TypeDeclarationSyntax>(x => x.IsKind(SyntaxKind.ClassDeclaration) || x.IsKind(SyntaxKind.InterfaceDeclaration));

            if (!GenerationHelpers.IsPartialType(typeDeclaration))
            {
                _analysisContext.AddDiagnostic(AsyncToSyncErrorCode.AttributeMustBeAppliedToPartialClass,
                                               "The [AsyncToSyncConverter] must be applied to a partial class.",
                                               target.GetLocation());

                return (false, null, null);
            }

            var parentClasses = target.Ancestors().Where(x => x.IsKind(SyntaxKind.ClassDeclaration)).OfType<ClassDeclarationSyntax>();
            if (parentClasses.Any(x => !GenerationHelpers.IsPartialType(x)))
            {
                _analysisContext.AddDiagnostic(AsyncToSyncErrorCode.AttributeMustBeAppliedInPartialClassHierarchy,
                                               "The [AsyncToSyncConverter] must be applied to a partial class nested in another partial class.",
                                               target.GetLocation());

                return (false, null, null);
            }

            var ns = GenerationHelpers.GetNamespace(typeDeclaration!);
            if (String.IsNullOrWhiteSpace(ns))
            {
                _analysisContext.AddDiagnostic(AsyncToSyncErrorCode.MethodMustBeInNameSpace,
                                               "The [AsyncToSyncConverter] must be applied to a method in a partial class contained in a namespace.",
                                               target.GetLocation());

                return (false, null, null);
            }

            // Return must be a task
            var returnTypeSymbol = _analysisContext.SemanticModel.GetSymbolInfo(target.ReturnType);
            //var s = context.SemanticModel.GetSymbolInfo(lfss.ReturnType);

            if (!IsTaskOrValueTask(returnTypeSymbol.Symbol))
            {
                _analysisContext.AddDiagnostic(AsyncToSyncErrorCode.MethodMustReturnTask,
                                               $"The [AsyncToSyncConverter] must be applied to a method which returns a task (Task or ValueTask) instead of type '{target.ReturnType.ToString()}'.",
                                               target.ReturnType.GetLocation());

                return (false, null, null);
            }

            return (true, typeDeclaration, ns);
        }

        private void AnalyzeReturnStatementMethodMustEndWithAsync(SyntaxNode node)
        {
            if (node is ReturnStatementSyntax returnStatement)
            {
                if (returnStatement.Expression is InvocationExpressionSyntax ies)
                {
                    var symbolInfo = _analysisContext.SemanticModel.GetSymbolInfo(ies);

                    if (symbolInfo.Symbol is IMethodSymbol ms)
                    {
                        if (IsTaskOrValueTask(ms.ReturnType))
                        {
                            if ((ies.Expression is not IdentifierNameSyntax ins
                                 || !ins.Identifier.ValueText.EndsWith("Async",
                                                                       StringComparison.Ordinal))
                                && !ms.Name.EndsWith("Async",
                                                     StringComparison.Ordinal))
                            {
                                _analysisContext.AddDiagnostic(AsyncToSyncErrorCode.ReturnedMethodMustEndWithAsync,
                                                               $"The returned method '{ms.Name}' must end with 'Async'.",
                                                               returnStatement.GetLocation());
                            }
                        }
                    }
                }
            }
        }

        private void AnalyzeAsyncForEachMustEndWithAsync(ForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword))
            {
                if (forEachStatement.Expression is InvocationExpressionSyntax ies)
                {
                    if (ies.Expression is IdentifierNameSyntax ins)
                    {
                        if (!GenerationHelpers.IdentifierEndsWithAsync(ins))
                        {
                            _analysisContext.AddDiagnostic(AsyncToSyncErrorCode.AwaitedMethodMustEndWithAsync,
                                                           $"The awaited method '{ins.Identifier.ValueText}' must end with 'Async'.",
                                                           ies.GetLocation());

                        }
                    }
                }
            }
        }

        private void AnalyzeAwaitedMethodsMustEndWithAsync(AwaitExpressionSyntax awaitSyntax)
        {
            if (awaitSyntax.Expression is InvocationExpressionSyntax invocation)
            {
                var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;

                if (memberAccess?.Expression is InvocationExpressionSyntax innerInvocation)
                {
                    if (innerInvocation.Expression is IdentifierNameSyntax ins)
                    {
                        if (!GenerationHelpers.IdentifierEndsWithAsync(ins))
                        {
                            AddDiagnosticToAnalysis(ins.Identifier.ValueText);
                        }
                    }
                }
                else if (invocation.Expression is IdentifierNameSyntax ins2)
                {
                    if (!GenerationHelpers.IdentifierEndsWithAsync(ins2))
                    {
                        AddDiagnosticToAnalysis(ins2.Identifier.ValueText);
                    }
                }

                void AddDiagnosticToAnalysis(string methodName)
                {
                    _analysisContext.AddDiagnostic(AsyncToSyncErrorCode.AwaitedMethodMustEndWithAsync,
                                                   $"The awaited method '{methodName}' must end with 'Async'.",
                                                   awaitSyntax.GetLocation());
                }
            }
        }
    }
}
