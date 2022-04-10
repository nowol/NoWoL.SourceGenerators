using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoWoL.SourceGenerators
{
    internal class AsyncToSyncConverter
    {
        internal const string DiagnosticCategory = "Async/Await remover generator";

        public (bool Success, MethodDeclarationSyntax Node, string? NameSpace, ClassDeclarationSyntax? ClassDeclaration) Transform(SourceProductionContext context, Compilation compilation, MethodDeclarationSyntax node)
        {
            var semanticModel = compilation.GetSemanticModel(node.SyntaxTree);

            var analysis = Analyze(context, compilation, semanticModel, node);

            if (analysis.ContainsDiagnosticErrors)
            {
                return (false, node, null, null);
            }

            if (compilation.GetDiagnostics().Any(x => x.Severity == DiagnosticSeverity.Error))
            {

            }

            var unmodifiedNode = node;

            var d = node.DescendantNodesAndSelf();
            
            node = node.ReplaceSyntax(d,
                                      (originalNode, potentiallyRewrittenNode) => ComputeReplacementNode(context, analysis, originalNode, potentiallyRewrittenNode),
                                      Enumerable.Empty<SyntaxToken>(),
                                      [ExcludeFromCodeCoverage]
                                      (token, _) => token,
                                      Enumerable.Empty<SyntaxTrivia>(),
                                      [ExcludeFromCodeCoverage]
                                      (trivia, _) => trivia);

            node = RemoveAttributes(node,
                                    unmodifiedNode,
                                    analysis);

            node = AddCodeGenAttribute(node);

            if (unmodifiedNode.HasLeadingTrivia)
            {
                node = node.WithLeadingTrivia(unmodifiedNode.GetLeadingTrivia());
            }

            return (true, node, analysis.NameSpace, analysis.ClassDeclaration);
        }

        private MethodDeclarationSyntax AddCodeGenAttribute(MethodDeclarationSyntax node)
        {
            return node.AddAttributeLists(CodeGenAttributeList);
        }

        private static readonly AttributeListSyntax CodeGenAttributeList = GetCodeGenAttribute();

        private static AttributeListSyntax GetCodeGenAttribute()
        {
            var attSyntax = SyntaxFactory.Attribute(
                                                   SyntaxFactory.QualifiedName(
                                                                               SyntaxFactory.QualifiedName(
                                                                                                           SyntaxFactory.QualifiedName(
                                                                                                                                       SyntaxFactory.IdentifierName("System"),
                                                                                                                                       SyntaxFactory.IdentifierName("CodeDom")),
                                                                                                           SyntaxFactory.IdentifierName("Compiler")),
                                                                               SyntaxFactory.IdentifierName(nameof(AsyncToSyncConverterGenerator))))
                                        .WithArgumentList(
                                                          SyntaxFactory.AttributeArgumentList(
                                                                                              SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(
                                                                                                                                                   new SyntaxNodeOrToken[]{
                                                                                                                                                        SyntaxFactory.AttributeArgument(
                                                                                                                                                         SyntaxFactory.LiteralExpression(
                                                                                                                                                          SyntaxKind.StringLiteralExpression,
                                                                                                                                                          SyntaxFactory.Literal("ExceptionGenerator"))),
                                                                                                                                                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                                                                                                                        SyntaxFactory.AttributeArgument(
                                                                                                                                                         SyntaxFactory.LiteralExpression(
                                                                                                                                                          SyntaxKind.StringLiteralExpression,
                                                                                                                                                          SyntaxFactory.Literal(typeof(AsyncToSyncConverterGenerator).Assembly.GetName().Version.ToString())))})));
            var als = SyntaxFactory.AttributeList().AddAttributes(attSyntax);

            return als;
        }

        private SyntaxNode ComputeReplacementNode(SourceProductionContext context, AsyncRemoverAnalysis analysis, SyntaxNode originalNode, SyntaxNode potentiallyRewrittenNode)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (potentiallyRewrittenNode is ReturnStatementSyntax returnStatement
                && returnStatement.Expression != null)
            {
                if (analysis.NodeMapping.ContainsKey(originalNode))
                {
                    return SyntaxFactory.ExpressionStatement(returnStatement.Expression).WithTriviaFrom(potentiallyRewrittenNode);
                }
            }

            if (analysis.NodeMapping.ContainsKey(originalNode))
            {
                return analysis.NodeMapping[originalNode].WithTriviaFrom(potentiallyRewrittenNode);
            }

            if (potentiallyRewrittenNode is AwaitExpressionSyntax awaitSyntax
                && awaitSyntax.Expression is InvocationExpressionSyntax invocation)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess
                    &&
                    (
                        string.Equals(memberAccess.Name.Identifier.ValueText,
                                      "ConfigureAwait",
                                      StringComparison.Ordinal)
                        || string.Equals(memberAccess.Name.Identifier.ValueText,
                                         "ConfigureAwaitWithCulture",
                                         StringComparison.Ordinal)
                    )
                    && memberAccess.Expression is InvocationExpressionSyntax inv)
                {
                    if (inv.Expression is IdentifierNameSyntax identifierNameSyntax)
                    {
                        var ss = inv.WithExpression(RemoveAsyncFrom(analysis,
                                                                    identifierNameSyntax));

                        return ss.WithTriviaFrom(awaitSyntax);
                    }

                    return inv.WithTriviaFrom(awaitSyntax);
                }
                else if (invocation.Expression is IdentifierNameSyntax ins)
                {
                    var ss = invocation.WithExpression(RemoveAsyncFrom(analysis,
                                                                       ins));

                    return ss.WithTriviaFrom(awaitSyntax);
                }

                return invocation.WithTriviaFrom(awaitSyntax);
            }

            // predicates
            if (potentiallyRewrittenNode is ParenthesizedLambdaExpressionSyntax parenthesizedLambdaSyntax)
            {
                if (parenthesizedLambdaSyntax.Body is InvocationExpressionSyntax ies)
                {
                    if (ies.Expression is IdentifierNameSyntax ins)
                    {
                        var newInvocation = ies.WithExpression(RemoveAsyncFrom(analysis,
                                                                               ins));
                        parenthesizedLambdaSyntax = parenthesizedLambdaSyntax.WithBody(newInvocation);
                    }
                }

                return parenthesizedLambdaSyntax.WithAsyncKeyword(default).WithTriviaFrom(potentiallyRewrittenNode);
            }

            // local function
            if (potentiallyRewrittenNode is LocalFunctionStatementSyntax localFct)
            {
                var asyncModifierIndex = localFct.Modifiers.IndexOf(SyntaxKind.AsyncKeyword);

                if (asyncModifierIndex != -1)
                {
                    localFct = localFct.WithModifiers(localFct.Modifiers.RemoveAt(asyncModifierIndex));
                }

                var result = RemoveAsyncFrom(analysis,
                                             localFct.Identifier,
                                             forceRename: true);

                return localFct.WithIdentifier((SyntaxToken)result.ReplacementNode!).WithTriviaFrom(potentiallyRewrittenNode);
            }

            // async stream - foreach
            if (potentiallyRewrittenNode is ForEachStatementSyntax forEachStatement
                && forEachStatement.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword))
            {
                return forEachStatement.WithAwaitKeyword(SyntaxFactory.Token(SyntaxKind.None))
                                       .WithTriviaFrom(potentiallyRewrittenNode);
            }

            // rewrite method definition
            if (potentiallyRewrittenNode is MethodDeclarationSyntax mds)
            {
                var ss = mds.WithReturnType(SyntaxFactory.IdentifierName(analysis.ReturnType!));

                var asyncModifier = ss.Modifiers.FirstOrDefault(x => x.IsKind(SyntaxKind.AsyncKeyword));

                if (asyncModifier.IsKind(SyntaxKind.AsyncKeyword))
                {
                    ss = ss.WithModifiers(ss.Modifiers.Remove(asyncModifier));
                }

                ss = RenameMethod(ss);

                return ss;
            }

            return potentiallyRewrittenNode;
        }

        private static IdentifierNameSyntax RemoveAsyncFrom(AsyncRemoverAnalysis analysis, IdentifierNameSyntax ins, bool forceRename = false)
        {
            if (!forceRename && analysis.IdentifiersThatDoesNotNeedRenaming.Contains(ins.Identifier.ValueText))
            {
                return ins;
            }

            return ins.WithIdentifier(SyntaxFactory.Identifier(GenerationHelpers.RemoveLastWord(ins.Identifier!.ValueText!,
                                                                                                "Async")));
        }

        private static (bool Success, SyntaxToken? ReplacementNode) RemoveAsyncFrom(AsyncRemoverAnalysis analysis, SyntaxToken token, bool forceRename = false)
        {
            if (!forceRename && analysis.IdentifiersThatDoesNotNeedRenaming.Contains(token.ValueText))
            {
                return (false, null);
            }

            var value = GenerationHelpers.RemoveLastWord(token.ValueText, "Async");

            return (true, SyntaxFactory.Identifier(value));
        }

        class AsyncRemoverAnalysis
        {
            private readonly Dictionary<string, INamedTypeSymbol?> _metadataSymbolMap = new();
            private readonly Compilation _compilation;
            private readonly SemanticModel _semanticModel;
            private readonly MethodDeclarationSyntax _target;

            public AsyncRemoverAnalysis(Compilation compilation, SemanticModel semanticModel, MethodDeclarationSyntax target)
            {
                _compilation = compilation;
                _semanticModel = semanticModel;
                _target = target;
            }

            public HashSet<string> IdentifiersThatDoesNotNeedRenaming { get; } = new();
            public Dictionary<SyntaxNode, SyntaxNode> NodeMapping { get; } = new();
            public bool ContainsDiagnosticErrors { get; set; }
            public string? ReturnType { get; set; }
            public string? NameSpace { get; set; }
            public ClassDeclarationSyntax? ClassDeclaration { get; set; }

            public INamedTypeSymbol? GetTypeByMetadataName(string fullyQualifiedMetadataName)
            {
                if (!_metadataSymbolMap.ContainsKey(fullyQualifiedMetadataName))
                {
                    _metadataSymbolMap[fullyQualifiedMetadataName] = _compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
                }

                return _metadataSymbolMap[fullyQualifiedMetadataName];
            }

            private List<AttributeSyntax>? _attributesToRemove = null;

            public List<AttributeSyntax> GetAttributesToRemove()
            {
                return _attributesToRemove!;
            }

            public void LoadAttributesToRemove()
            {
                _attributesToRemove = new();

                foreach (AttributeListSyntax attributeListSyntax in _target.AttributeLists)
                {
                    foreach (var (attributeSyntax, attributeSymbol) in FilterAttributes(attributeListSyntax))
                    {
                        INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                        string fullName = attributeContainingTypeSymbol.ToDisplayString();

                        // Is the attribute the [ExceptionGenerator] attribute?
                        if (fullName == AsyncToSyncConverterGenerator.AsyncToSyncConverterAttributeFqn)
                        {
                            _attributesToRemove.Add(attributeSyntax);
                        }
                    }
                }

                List<(AttributeSyntax, IMethodSymbol)> FilterAttributes(AttributeListSyntax attributeListSyntax)
                {
                    var results = new List<(AttributeSyntax, IMethodSymbol)>();

                    foreach (var attributeSyntax in attributeListSyntax.Attributes)
                    {
                        if (ModelExtensions.GetSymbolInfo(_semanticModel,
                                                          attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                        {
                            // weird, we couldn't get the symbol, ignore it
                            continue;
                        }

                        results.Add((attributeSyntax, attributeSymbol));
                    }

                    return results;
                }
            }
        }

        private AsyncRemoverAnalysis Analyze(SourceProductionContext context, Compilation compilation, SemanticModel semanticModel, MethodDeclarationSyntax node)
        {
            var d = node.DescendantNodesAndSelf();
            var analysis = new AsyncRemoverAnalysis(compilation, semanticModel, node);

            AnalyzeAncestors(context, 
                             analysis,
                             semanticModel,
                             node);

            analysis.LoadAttributesToRemove();

            foreach (var syntaxNode in d)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                if (syntaxNode is MemberAccessExpressionSyntax mae)
                {
                    var maeSymbol = semanticModel.GetSymbolInfo(mae).Symbol;

                    if (maeSymbol != null
                        && maeSymbol.ContainingType != null
                        && maeSymbol.ContainingType.ToString() == "System.Threading.Tasks.Task"
                        && maeSymbol.Name == nameof(System.Threading.Tasks.Task.Delay))
                    {
                        var nn = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                      SyntaxFactory.ParseExpression("System.Threading.Thread"),
                                                                      SyntaxFactory.IdentifierName(nameof(System.Threading.Thread.Sleep)));
                        analysis.NodeMapping.Add(syntaxNode, nn);
                    }
                }

                if (syntaxNode is ArgumentListSyntax als)
                {
                    if (als.Parent is InvocationExpressionSyntax invocation)
                    {
                        var invocationSymbol = semanticModel.GetSymbolInfo(invocation).Symbol;

                        if (invocationSymbol != null
                            && invocationSymbol.ContainingType != null
                            && invocationSymbol.ContainingType.ToString() == "System.Threading.Tasks.Task"
                            && invocationSymbol.Name == nameof(System.Threading.Tasks.Task.Delay))
                        {
                            if (als.Arguments.Count == 2) // last param is cancel token and we need to remove it
                            {
                                analysis.NodeMapping.Add(syntaxNode, SyntaxFactory.ArgumentList(als.Arguments.RemoveAt(1)));
                            }
                        }
                    }
                }

                if (syntaxNode is IdentifierNameSyntax ins)
                {
                    if (ins.Identifier.ValueText != null
                        && ins.Identifier.ValueText.EndsWith("Async") == true)
                    {
                        var result = RemoveAsyncFrom(analysis, ins.Identifier);

                        if (result.Success)
                        {
                            analysis.NodeMapping.Add(syntaxNode,
                                                     SyntaxFactory.IdentifierName((SyntaxToken)(result.ReplacementNode!)));
                        }
                    }
                    else if (syntaxNode.Parent != null)
                    {
                        if (syntaxNode.Parent is MethodDeclarationSyntax
                            || syntaxNode.Parent is LocalFunctionStatementSyntax)
                        {
                            var symbol3 = semanticModel.GetSymbolInfo(syntaxNode).Symbol;
                            if (IsTaskOrValueTask(symbol3))
                            {
                                analysis.NodeMapping.Add(syntaxNode, SyntaxFactory.IdentifierName("void"));
                            }
                        }
                    }
                }

                if (syntaxNode is GenericNameSyntax)
                {
                    if (syntaxNode.Parent != null)
                    {
                        if (syntaxNode.Parent is MethodDeclarationSyntax
                            || syntaxNode.Parent is LocalFunctionStatementSyntax)
                        {
                            var symbol33 = semanticModel.GetSymbolInfo(syntaxNode).Symbol;

                            if (IsTaskOrValueTask(symbol33))
                            {
                                var argType = syntaxNode.ChildNodes().OfType<TypeArgumentListSyntax>().FirstOrDefault();

                                if (argType != null
                                    && argType.Arguments.Count > 0)
                                {
                                    analysis.NodeMapping.Add(syntaxNode, argType.Arguments.First());
                                }
                            }
                        }
                    }
                }

                if (syntaxNode is ReturnStatementSyntax returnStatement)
                {
                    if (returnStatement.Expression is InvocationExpressionSyntax ies)
                    {
                        if (semanticModel.GetSymbolInfo(ies).Symbol is IMethodSymbol ms)
                        {
                            if (ms.ReturnType is INamedTypeSymbol ints 
                                && ints.TypeArguments.Length == 0
                                && IsTaskOrValueTask(ms.ReturnType))
                            {
                                analysis.NodeMapping.Add(syntaxNode, ies);
                            }
                        }
                    }

                    AnalyzeMethodMustEndWithAsync(context,
                                                  analysis,
                                                  semanticModel,
                                                  returnStatement);
                }

                if (syntaxNode is LocalFunctionStatementSyntax localFct)
                {
                    foreach (var parameterSyntax in localFct.ParameterList.Parameters)
                    {
                        analysis.IdentifiersThatDoesNotNeedRenaming.Add(parameterSyntax.Identifier.ValueText);
                    }
                }

                if (syntaxNode is ParameterListSyntax pls
                    && pls.Parameters.Count > 0)
                {
                    var newParameters = new SeparatedSyntaxList<ParameterSyntax>();
                    bool wasModified = false;

                    foreach (var parameterSyntax in pls.Parameters)
                    {
                        bool parameterWasConverted = false;

                        analysis.IdentifiersThatDoesNotNeedRenaming.Add(parameterSyntax.Identifier.ValueText);
                    
                        var symbol4 = semanticModel.GetSymbolInfo(parameterSyntax.Type!).Symbol;

                        if (symbol4 != null
                            && symbol4.ContainingNamespace != null 
                            && symbol4.ContainingNamespace.ToString() == "System"
                            && symbol4.Name == "Func")
                        {
                            var argType = parameterSyntax.Type!.DescendantNodes(x => !x.IsKind(SyntaxKind.TypeArgumentList)).OfType<TypeArgumentListSyntax>().FirstOrDefault();

                            if (argType != null
                                && argType.Arguments.Count > 0)
                            {
                                var lastArg = argType.Arguments.Last();
                                var lastArgSymbol = semanticModel.GetSymbolInfo(lastArg).Symbol;

                                if (IsTaskOrValueTask(lastArgSymbol)
                                    && lastArgSymbol is INamedTypeSymbol nts)
                                {
                                    var newParam = parameterSyntax;

                                    if (nts.TypeArguments.Length == 0)
                                    {
                                        if (argType.Arguments.Count == 1)
                                        {
                                            newParam = newParam.WithType(SyntaxFactory.ParseTypeName("System.Action"));
                                        }
                                        else
                                        {
                                            var newArgs = argType.Arguments.RemoveAt(argType.Arguments.Count - 1);
                                            var actParam = SyntaxFactory.GenericName(SyntaxFactory.Identifier("System.Action"),
                                                                                     SyntaxFactory.TypeArgumentList(newArgs));

                                            newParam = newParam.WithType(actParam);
                                        }
                                    }
                                    else
                                    {
                                        var newArgs = argType.Arguments.RemoveAt(argType.Arguments.Count - 1);
                                        newArgs = newArgs.Add(
                                                              SyntaxFactory.ParseTypeName(nts.TypeArguments[0].ToString())
                                                              );
                                        var actParam = SyntaxFactory.GenericName(SyntaxFactory.Identifier("System.Func"),
                                                                                 SyntaxFactory.TypeArgumentList(newArgs));

                                        newParam = newParam.WithType(actParam);
                                    }

                                    wasModified = true;
                                    parameterWasConverted = true;

                                    newParameters = newParameters.Add(newParam);
                                }
                            }
                        }

                        if (!parameterWasConverted)
                        {
                            newParameters = newParameters.Add(parameterSyntax);
                        }
                    }

                    if (wasModified)
                    {
                        analysis.NodeMapping.Add(pls, pls.WithParameters(newParameters));
                    }
                }

                if (syntaxNode is AwaitExpressionSyntax awaitStatement)
                {
                    AnalyzeAwaitedMethodsMustEndWithAsync(context,
                                                          analysis,
                                                          semanticModel,
                                                          awaitStatement);
                }

                if (syntaxNode is ForEachStatementSyntax forEachStatement)
                {
                    AnalyzeAsyncForEachMustEndWithAsync(context,
                                                        analysis,
                                                        semanticModel,
                                                        forEachStatement);
                }
            }

            return analysis;
        }

        private bool AnalyzeAsyncForEachMustEndWithAsync(SourceProductionContext context, AsyncRemoverAnalysis analysis, SemanticModel semanticModel, ForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword))
            {
                if (forEachStatement.Expression is InvocationExpressionSyntax ies)
                {
                    if (ies.Expression is IdentifierNameSyntax ins)
                    {
                        if (!GenerationHelpers.IdentifierEndsWithAsync(ins))
                        {
                            AddDiagnostic(context,
                                          analysis,
                                          AsyncToSyncErrorCode.AwaitedMethodMustEndWithAsync,
                                          $"The awaited method '{ins.Identifier.ValueText}' must end with 'Async'.",
                                          ies.GetLocation());

                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool AnalyzeAwaitedMethodsMustEndWithAsync(SourceProductionContext context, AsyncRemoverAnalysis analysis, SemanticModel semanticModel, AwaitExpressionSyntax awaitStatement)
        {
            if (awaitStatement.Expression is InvocationExpressionSyntax invocation)
            {
                var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;

                if (memberAccess?.Expression is InvocationExpressionSyntax innerInvocation)
                {
                    if (innerInvocation.Expression is IdentifierNameSyntax ins)
                    {
                        if (!GenerationHelpers.IdentifierEndsWithAsync(ins)
                            && !analysis.IdentifiersThatDoesNotNeedRenaming.Contains(ins.Identifier.ValueText))
                        {
                            AddDiagnosticToAnalysis(ins.Identifier.ValueText);
                            return false;
                        }
                    }
                }
                else if (invocation.Expression is IdentifierNameSyntax ins2)
                {
                    if (!GenerationHelpers.IdentifierEndsWithAsync(ins2)
                        && !analysis.IdentifiersThatDoesNotNeedRenaming.Contains(ins2.Identifier.ValueText))
                    {
                        AddDiagnosticToAnalysis(ins2.Identifier.ValueText);
                        return false;
                    }
                }

                void AddDiagnosticToAnalysis(string methodName)
                {
                    AddDiagnostic(context,
                                  analysis,
                                  AsyncToSyncErrorCode.AwaitedMethodMustEndWithAsync,
                                  $"The awaited method '{methodName}' must end with 'Async'.",
                                  awaitStatement.GetLocation());
                }
            }

            return true;
        }

        private bool AnalyzeMethodMustEndWithAsync(SourceProductionContext context, AsyncRemoverAnalysis analysis, SemanticModel semanticModel, ReturnStatementSyntax returnStatement)
        {
            if (returnStatement.Expression is InvocationExpressionSyntax ies)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(ies);
                if (symbolInfo.Symbol is IMethodSymbol ms)
                {
                    var result = GetReturnsTypeDetails(ms.ReturnType,
                                                  analysis);

                    if (result.ReturnsTask)
                    {
                        if ((ies.Expression is not IdentifierNameSyntax ins
                             || !ins.Identifier.ValueText.EndsWith("Async",
                                                                   StringComparison.Ordinal))
                            && !ms.Name.EndsWith("Async",
                                                 StringComparison.Ordinal))
                        {
                            AddDiagnostic(context,
                                          analysis,
                                          AsyncToSyncErrorCode.ReturnedMethodMustEndWithAsync,
                                          $"The returned method '{ms.Name}' must end with 'Async'.",
                                          returnStatement.GetLocation());

                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool AnalyzeAncestors(SourceProductionContext context, AsyncRemoverAnalysis analysis, SemanticModel semanticModel, MethodDeclarationSyntax target)
        {
            if (!target.Identifier.ValueText.EndsWith("Async", StringComparison.OrdinalIgnoreCase))
            {
                AddDiagnostic(context,
                              analysis,
                              AsyncToSyncErrorCode.AttributeMustBeAppliedToAClassEndingWithAsync,
                              "The [AsyncToSyncConverter] must be applied to a method ending with Async.",
                              target.GetLocation());
                return false;
            }

            analysis.ClassDeclaration = target.FirstAncestorOrSelf<ClassDeclarationSyntax>();

            if (!GenerationHelpers.IsPartialClass(analysis.ClassDeclaration))
            {
                AddDiagnostic(context,
                              analysis,
                              AsyncToSyncErrorCode.AttributeMustBeAppliedToPartialClass,
                              "The [AsyncToSyncConverter] must be applied to a partial class.",
                              target.GetLocation());

                return false;
            }

            var parentClasses = target.Ancestors().Where(x => x.IsKind(SyntaxKind.ClassDeclaration)).OfType<ClassDeclarationSyntax>();
            if (parentClasses.Any(x => !GenerationHelpers.IsPartialClass(x)))
            {
                AddDiagnostic(context,
                              analysis,
                              AsyncToSyncErrorCode.AttributeMustBeAppliedInPartialClassHierarchy,
                              "The [AsyncToSyncConverter] must be applied to a partial class nested in another partial class.",
                              target.GetLocation());

                return false;
            }

            analysis.NameSpace = GenerationHelpers.GetNamespace(analysis.ClassDeclaration!);
            if (String.IsNullOrWhiteSpace(analysis.NameSpace))
            {
                AddDiagnostic(context,
                              analysis,
                              AsyncToSyncErrorCode.MethodMustBeInNameSpace,
                              "The [AsyncToSyncConverter] must be applied to a method in a partial class contained in a namespace.",
                              target.GetLocation());

                return false;
            }

            // Return must be a task
            var returnTypeSymbol = ModelExtensions.GetTypeInfo(semanticModel,
                                                               target.ReturnType);

            if (returnTypeSymbol.Type != null)
            {
                var r = GetReturnsTypeDetails(returnTypeSymbol.Type,
                                              analysis);
                analysis.ReturnType = r.NewDataType;

                if (String.IsNullOrWhiteSpace(r.NewDataType))
                {
                    AddDiagnostic(context,
                                  analysis,
                                  AsyncToSyncErrorCode.MethodMustReturnTask,
                                  $"The [AsyncToSyncConverter] must be applied to a method return a task instead of type '{target.ReturnType.ToString()}'.",
                                  target.ReturnType.GetLocation());

                    return false;
                }
            }

            return true;
        }

        private bool IsTaskOrValueTask(ISymbol? symbol)
        {
            return symbol != null
                   && symbol.ContainingNamespace != null
                   && symbol.ContainingNamespace.ToString() == "System.Threading.Tasks"
                   && (symbol.Name == nameof(System.Threading.Tasks.Task) || symbol.Name == nameof(System.Threading.Tasks.ValueTask));
        }

        private MethodDeclarationSyntax RemoveAttributes(MethodDeclarationSyntax target, 
                                                         MethodDeclarationSyntax unmodifiedNode, 
                                                         AsyncRemoverAnalysis analysis)
        {
            var newAttributeList = new List<AttributeListSyntax>();

            foreach (var attributeListSyntax in unmodifiedNode.AttributeLists)
            {
                var attList = attributeListSyntax;

                foreach (var attributeSyntax in analysis.GetAttributesToRemove())
                {
                    attList = attList.WithAttributes(attList.Attributes.Remove(attributeSyntax));
                }

                if (attList.Attributes.Count > 0)
                {
                    newAttributeList.Add(attList);
                }
            }

            return target.WithAttributeLists(new SyntaxList<AttributeListSyntax>(newAttributeList));
        }

        private (bool ReturnsTask, bool ReturnsVoid, string NewDataType) GetReturnsTypeDetails(ITypeSymbol returnTypeSymbol, AsyncRemoverAnalysis analysis)
        {
            var nonGenericTaskClass = analysis.GetTypeByMetadataName("System.Threading.Tasks.Task");
            if (returnTypeSymbol.Equals(nonGenericTaskClass, SymbolEqualityComparer.Default) == true)
            {
                return (true, true, "void");
            }

            var nonGenericValueTaskClass = analysis.GetTypeByMetadataName("System.Threading.Tasks.ValueTask");
            if (returnTypeSymbol.Equals(nonGenericValueTaskClass, SymbolEqualityComparer.Default) == true)
            {
                return (true, true, "void");
            }

            if (returnTypeSymbol is INamedTypeSymbol genericType)
            {
                // Task<>
                var genericTaskClass = analysis.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
                if (genericType.OriginalDefinition.Equals(genericTaskClass, SymbolEqualityComparer.Default))
                {
                    return (true, false, genericType.TypeArguments.First().ToString());
                }

                // ValueTask<>
                var genericValueTaskClass = analysis.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1");
                if (genericType.OriginalDefinition.Equals(genericValueTaskClass, SymbolEqualityComparer.Default))
                {
                    return (true, false, genericType.TypeArguments.First().ToString());
                }
            }

            return (false, false, String.Empty);
        }

        public MethodDeclarationSyntax RenameMethod(MethodDeclarationSyntax node)
        {
            return node.WithIdentifier(SyntaxFactory.Identifier(GenerationHelpers.RemoveLastWord(node.Identifier!.ValueText!,
                                                                                                 "Async")));
        }

        private void AddDiagnostic(SourceProductionContext context, AsyncRemoverAnalysis analysis, AsyncToSyncErrorCode errorCode, string message, Location location)
        {
            analysis.ContainsDiagnosticErrors = true;
            context.ReportDiagnostic(Diagnostic.Create(GenerationHelpers.ConvertErrorCode(errorCode),
                                                                                          DiagnosticCategory,
                                                                                          message,
                                                                                          defaultSeverity: DiagnosticSeverity.Error,
                                                                                          severity: DiagnosticSeverity.Error,
                                                                                          isEnabledByDefault: true,
                                                                                          warningLevel: 0,
                                                                                          location: location));
        }
    }
}
