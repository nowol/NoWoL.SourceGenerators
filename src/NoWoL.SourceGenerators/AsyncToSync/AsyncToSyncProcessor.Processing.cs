using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoWoL.SourceGenerators
{
    internal partial class AsyncToSyncProcessor // Processing
    {
        public void ProcessNode(SyntaxNode node)
        {
            _analysisContext.CancellationToken.ThrowIfCancellationRequested();
            
            bool processChildNodes = true;
            var symbolizer = new Symbolizer(_analysisContext.SemanticModel, node);

            if (node.IsKind(SyntaxKind.Block))
            {
                //processChildNodes = true;
            }
            else if (node.IsKind(SyntaxKind.IdentifierName)
                     && node.Parent is InvocationExpressionSyntax
                     && node is IdentifierNameSyntax ins)
            {
                AppendWithTrivia(RemoveAsyncFrom(ins).WithTriviaFrom(ins));

                processChildNodes = false;
            }
            else if (node.IsKind(SyntaxKind.IdentifierName)
                     && node.Parent is ArgumentSyntax
                     && node is IdentifierNameSyntax argSyntax)
            {
                AppendWithTrivia(RemoveAsyncFrom(argSyntax).WithTriviaFrom(argSyntax));

                processChildNodes = false;
            }
            else if (node.IsKind(SyntaxKind.IdentifierName)
                     && node.Parent is LocalFunctionStatementSyntax
                     && node is IdentifierNameSyntax argSyntax2)
            {
                if (symbolizer.SymbolInfo.Symbol is ITypeSymbol ts)
                {
                    AnalyzeReturnStatementMethodMustEndWithAsync(node);

                    ProcessReturnStatementSyntax(node,
                                                 ts);

                    processChildNodes = false;
                }
            }
            else if (node.IsKind(SyntaxKind.GenericName)
                     &&
                     (
                         node.Parent is MethodDeclarationSyntax
                         ||
                         node.Parent is LocalFunctionStatementSyntax
                     ))
            {
                if (symbolizer.SymbolInfo.Symbol is ITypeSymbol ts)
                {
                    AnalyzeReturnStatementMethodMustEndWithAsync(node);

                    ProcessReturnStatementSyntax(node,
                                                 ts);
                    processChildNodes = false;
                }
            }
            else if (node.IsKind(SyntaxKind.PredefinedType)
                     &&
                     (
                         node.Parent is MethodDeclarationSyntax
                         ||
                         node.Parent is LocalFunctionStatementSyntax
                     ))
            {
                if (symbolizer.SymbolInfo.Symbol is ITypeSymbol ts)
                {
                    AnalyzeReturnStatementMethodMustEndWithAsync(node);

                    ProcessReturnStatementSyntax(node,
                                                 ts);
                    processChildNodes = false;
                }
            }
            else if (node.IsKind(SyntaxKind.IdentifierName)
                     &&
                     (
                         node.Parent is MethodDeclarationSyntax
                         ||
                         node.Parent is LocalFunctionStatementSyntax
                     ))
            {
                if (symbolizer.SymbolInfo.Symbol is ITypeSymbol ts)
                {
                    AnalyzeReturnStatementMethodMustEndWithAsync(node);
                    ProcessReturnStatementSyntax(node,
                                                 ts);
                    processChildNodes = false;
                }
            }
            else if (node.IsKind(SyntaxKind.ReturnStatement)
                     && node is ReturnStatementSyntax rss
                     && rss.Expression != null)
            {

                AnalyzeReturnStatementMethodMustEndWithAsync(node);

                bool removeReturn = false;
                foreach (var ancestor in rss.Ancestors())
                {
                    SyntaxNode? returnTypeToAnalyze = null;

                    if (ancestor is LocalFunctionStatementSyntax lfss)
                    {
                        returnTypeToAnalyze = lfss.ReturnType;
                    }
                    else if (ancestor is MethodDeclarationSyntax mds)
                    {
                        returnTypeToAnalyze = mds.ReturnType;
                    }

                    if (returnTypeToAnalyze != null)
                    {
                        var s = _analysisContext.SemanticModel.GetSymbolInfo(returnTypeToAnalyze);

                        if (IsTaskOrValueTask(s.Symbol))
                        {
                            var argType = returnTypeToAnalyze.DescendantNodes(x => !x.IsKind(SyntaxKind.TypeArgumentList)).OfType<TypeArgumentListSyntax>().FirstOrDefault();

                            removeReturn = argType == null || argType.Arguments.Count == 0;
                        }

                        break;
                    }
                }

                if (removeReturn)
                {
                    foreach (var childNodesAndToken in rss.ChildNodesAndTokens())
                    {
                        if (childNodesAndToken.IsToken)
                        {
                            var token = childNodesAndToken.AsToken()!;

                            if (token.IsKind(SyntaxKind.ReturnKeyword))
                            {
                                AddLeadingTrivia(token);
                            }
                            else
                            {
                                ProcessToken(token);
                            }
                        }
                        else
                        {
                            ProcessNode(childNodesAndToken.AsNode()!);
                        }
                    }

                    processChildNodes = false;
                }
            }
            else if (node.IsKind(SyntaxKind.AttributeList))
            {
                processChildNodes = false;
                ProcessAttributeList(node);
            }
            else if (IsCancellationTokenRelated(node))
            {
                processChildNodes = false;
            }
            //else if (node.IsKind(SyntaxKind.LocalFunctionStatement)
            //         && node is LocalFunctionStatementSyntax lfss)
            //{
                
            //}
            else if (node.IsKind(SyntaxKind.ForEachStatement)
                     && node is ForEachStatementSyntax forEachStatement
                     && forEachStatement.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword))
            {
                AnalyzeAsyncForEachMustEndWithAsync(forEachStatement);

                processChildNodes = false;

                foreach (var child in node.ChildNodesAndTokens())
                {
                    if (child.IsKind(SyntaxKind.AwaitKeyword))
                    {
                        AddLeadingTrivia(node);
                        continue;
                    }

                    if (child.IsToken)
                    {
                        ProcessToken(child.AsToken()!);
                    }
                    else
                    {
                        ProcessNode(child.AsNode()!);
                    }
                }
            }
            else if (node.IsKind(SyntaxKind.AwaitExpression)
                     && node is AwaitExpressionSyntax awaitSyntax)
            {
                AnalyzeAwaitedMethodsMustEndWithAsync(awaitSyntax);

                var unfoldedExpr = UnfoldAwaitExpression(node);

                if (TryTaskDelay(unfoldedExpr!, out var argList))
                {
                    var nnn = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                   SyntaxFactory.ParseExpression("System.Threading.Thread"),
                                                                   SyntaxFactory.IdentifierName(nameof(System.Threading.Thread.Sleep)));

                    var inv4 = SyntaxFactory.InvocationExpression(nnn,
                                                                  argList!);

                    inv4 = inv4.WithTriviaFrom(node!);

                    _analysisContext.Builder.AddRaw(inv4.ToFullString());
                    processChildNodes = false;
                }
                else
                {
                    // todo: add test with 0 param, with 2 async params, with 2 async param and 1 non async, with async inside async

                    if (unfoldedExpr is InvocationExpressionSyntax ies7
                        && ies7.Expression is IdentifierNameSyntax ins7)
                    {
                        if (ies7.ArgumentList.Arguments.Count > 0)
                        {
                            var newArgList = ies7.ArgumentList;
                            IndentedStringBuilder? tempIsb = null;
                            AsyncToSyncProcessor? tempProcessor = null;

                            for (int i = 0, cpt = 0; i < ies7.ArgumentList.Arguments.Count; i++)
                            {
                                var arg = ies7.ArgumentList.Arguments[i];

                                if (IsCancellationTokenRelated(arg))
                                {
                                    newArgList = newArgList.WithArguments(newArgList.Arguments.RemoveAt(cpt));
                                }
                                else if (arg.Expression is LiteralExpressionSyntax)
                                {
                                    cpt++;
                                }
                                else
                                {
                                    if (tempProcessor == null)
                                    {
                                        tempIsb = new IndentedStringBuilder();
                                        tempProcessor = new AsyncToSyncProcessor(_analysisContext.Compilation, _analysisContext.SourceProductionContext, node, tempIsb);
                                    }

                                    tempProcessor.ProcessNode(arg);

                                    if (tempProcessor.ContainsDiagnosticErrors)
                                    {
                                        _analysisContext.ContainsDiagnosticErrors = true;
                                    }

                                    var newNode2 = CSharpSyntaxTree.ParseText(tempProcessor.GetBuilderContent());

                                    SyntaxNode ee = newNode2.GetRoot();

                                    ExpressionSyntax? newArgExpr = null; // find the first expression to be use as argument
                                    var bogusVariable = ee.DescendantNodes(sn =>
                                    {
                                        if (sn is ExpressionSyntax e)
                                        {
                                            newArgExpr = e;
                                            return false;
                                        }

                                        return true;
                                    }).ToList(); // the ToList is used to force the materialization of the query

                                    if (newArgExpr != null)
                                    {
                                        var newArgNode = SyntaxFactory.Argument(newArgExpr!);
                                        newArgList = newArgList.WithArguments(newArgList.Arguments.Replace(newArgList.Arguments[cpt], newArgNode));
                                    }

                                    tempIsb!.Clear(true);

                                    cpt++;
                                }
                            }

                            var newIdentifier = RemoveAsyncFrom(ins7);

                            var newNode = ies7.WithArgumentList(newArgList.WithTriviaFrom(ies7.ArgumentList))
                                              .WithExpression(newIdentifier)
                                              .WithTriviaFrom(node);

                            AppendWithTrivia(newNode);
                            processChildNodes = false;
                        }
                        else 
                        {
                            var newIdentifier = RemoveAsyncFrom(ins7);

                            var newNode = ies7.WithExpression(newIdentifier)
                                              .WithTriviaFrom(node);

                            AppendWithTrivia(newNode);
                            processChildNodes = false;
                        }
                    }
                }
            }
            else if (node.IsKind(SyntaxKind.ParameterList)
                     && node is ParameterListSyntax pls)
            {
                processChildNodes = false;

                if (pls.Parameters.Count == 0)
                {
                    AppendWithTrivia(pls);
                }
                else
                {
                    int cpt = pls.Parameters.Count - 1;

                    var newPls = pls;

                    while (cpt >= 0)
                    {
                        var ps = pls.Parameters[cpt];
                        var newPs = ProcessParameterSyntax(ps);

                        if (newPs != null)
                        {
                            var psToReplace = newPls.Parameters[cpt];
                            newPls = newPls.WithParameters(newPls.Parameters.Replace(psToReplace, newPs));
                        }
                        else
                        {
                            newPls = newPls.WithParameters(newPls.Parameters.RemoveAt(cpt));
                        }

                        cpt--;
                    }

                    AppendWithTrivia(newPls);
                }
            }

            if (processChildNodes)
            {
                foreach (var childNodesAndToken in node.ChildNodesAndTokens())
                {
                    if (childNodesAndToken.IsToken)
                    {
                        ProcessToken(childNodesAndToken.AsToken()!);
                    }
                    else
                    {
                        ProcessNode(childNodesAndToken.AsNode()!);
                    }
                }
            }
        }

        private void ProcessToken(SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.AsyncKeyword)
                || token.IsKind(SyntaxKind.AwaitKeyword))
            {
                AddLeadingTrivia(token);
            }
            else
            {
                if (token.IsKind(SyntaxKind.IdentifierToken)
                    &&
                    (
                        token.Parent is MethodDeclarationSyntax
                        ||
                        token.Parent is LocalFunctionStatementSyntax
                    ))
                {
                    AddLeadingTrivia(token);
                    _analysisContext.Builder.AddRaw(RemoveAsyncFrom(token).ToString());
                    AddTrailingTrivia(token);
                }
                else
                {
                    _analysisContext.Builder.AddRaw(token.ToFullString());
                }
            }
        }

        private void ProcessAttributeList(SyntaxNode n)
        {
            AddLeadingTrivia(n);

            var attList = n as AttributeListSyntax;

            foreach (var attribute in attList!.Attributes)
            {
                var isGenAttr = false;
                var attSymbol = _analysisContext.SemanticModel.GetSymbolInfo(attribute);

                if (attSymbol.Symbol is IMethodSymbol ms)
                {
                    if (ms.ContainingType.ToDisplayString() == AsyncToSyncConverterGenerator.AsyncToSyncConverterAttributeFqn)
                    {
                        isGenAttr = true;
                    }
                }

                if (isGenAttr)
                {
                    AddLeadingTrivia(attribute);

                    _analysisContext.Builder.AddRaw(CodeGenAttribute);

                    AddTrailingTrivia(attribute);
                }
                else
                {
                    _analysisContext.Builder.AddRaw(attList!.OpenBracketToken.ToString());

                    _analysisContext.Builder.AddRaw(attribute.ToFullString());

                    _analysisContext.Builder.AddRaw(attList!.CloseBracketToken.ToString());
                }
            }

            AddTrailingTrivia(n);
        }

        private void ProcessReturnStatementSyntax(SyntaxNode originalNode, ITypeSymbol rs)
        {
            var rtd = GetReturnsTypeDetails(rs, _analysisContext.Compilation);

            if (originalNode!.HasLeadingTrivia)
            {
                _analysisContext.Builder.AddRaw(originalNode.GetLeadingTrivia().ToFullString());
            }

            if (String.IsNullOrWhiteSpace(rtd))
            {
                _analysisContext.Builder.AddRaw(originalNode.ToString());
            }
            else
            {
                _analysisContext.Builder.AddRaw(rtd);
            }

            if (originalNode!.HasTrailingTrivia)
            {
                _analysisContext.Builder.AddRaw(originalNode.GetTrailingTrivia().ToFullString());
            }
        }

        private ParameterSyntax? ProcessParameterSyntax(ParameterSyntax parameterSyntax)
        {
            var symbol4 = _analysisContext.SemanticModel.GetSymbolInfo(parameterSyntax.Type!);
            var processedParameter = parameterSyntax.WithIdentifier(RemoveAsyncFrom(parameterSyntax.Identifier));

            if (symbol4.Symbol != null)
            {
                if (symbol4.Symbol.Equals(_analysisContext.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken"),
                                          SymbolEqualityComparer.Default))
                {
                    // do nothing to remove the cancellation token 
                    processedParameter = null;
                }
                else if (symbol4.Symbol.ContainingNamespace != null
                         && symbol4.Symbol.ContainingNamespace.ToString() == "System"
                         && symbol4.Symbol.Name == "Func")
                {
                    var argType = parameterSyntax!.Type!.DescendantNodes(x => !x.IsKind(SyntaxKind.TypeArgumentList)).OfType<TypeArgumentListSyntax>().FirstOrDefault();

                    if (argType != null)
                    {
                        if (argType.Arguments.Count > 0)
                        {
                            var lastArg = argType.Arguments.Last();
                            var lastArgSymbol = _analysisContext.SemanticModel.GetSymbolInfo(lastArg).Symbol;

                            if (IsTaskOrValueTask(lastArgSymbol)
                                && lastArgSymbol is INamedTypeSymbol nts)
                            {
                                var newParam = parameterSyntax.WithIdentifier(RemoveAsyncFrom(parameterSyntax.Identifier));

                                if (nts.TypeArguments.Length == 0) // Task
                                {
                                    if (argType.Arguments.Count == 1) // Convert Func<Task> to Action
                                    {
                                        newParam = newParam.WithType(SyntaxFactory.ParseTypeName("System.Action").WithTriviaFrom(parameterSyntax.Type));
                                    }
                                    else // Convert Func<x, Task> to Action<x>
                                    {
                                        var newArgs2 = RemoveCancellationToken(argType);
                                        var newArgs = newArgs2.Arguments.RemoveAt(newArgs2.Arguments.Count - 1);

                                        var actParam = SyntaxFactory.GenericName(SyntaxFactory.Identifier("System.Action"),
                                                                                 SyntaxFactory.TypeArgumentList(newArgs))
                                                                    .WithTriviaFrom(parameterSyntax.Type);

                                        newParam = newParam.WithType(actParam);
                                    }
                                }
                                else // Task<>
                                {
                                    var newArgs2 = RemoveCancellationToken(argType);
                                    var newArgs = newArgs2.Arguments.Replace(lastArg, SyntaxFactory.ParseTypeName(nts.TypeArguments[0].ToString()).WithTriviaFrom(lastArg));

                                    var actParam = SyntaxFactory.GenericName(SyntaxFactory.Identifier("System.Func"),
                                                                             SyntaxFactory.TypeArgumentList(newArgs))
                                                                .WithTriviaFrom(parameterSyntax.Type);

                                    newParam = newParam.WithType(actParam);
                                }

                                processedParameter = newParam.WithTriviaFrom(parameterSyntax);
                            }
                        }
                    }
                }
            }

            return processedParameter;
        }
    }
}
