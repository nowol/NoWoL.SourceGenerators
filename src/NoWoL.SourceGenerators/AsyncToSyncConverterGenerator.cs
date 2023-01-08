using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace NoWoL.SourceGenerators
{
    [Generator]
    public partial class AsyncToSyncConverterGenerator : IIncrementalGenerator
    {
        // This class was inspired from NetEscapades.EnumGenerators
        // https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/

        internal const string AsyncToSyncConverterAttributeFqn = "NoWoL.SourceGenerators.AsyncToSyncConverterAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource("AsyncToSyncConverterAttributeFqn.g.cs",
                                                                          SourceText.From(EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoader).Assembly, 
                                                                                                                     EmbeddedResourceLoader.AsyncToSyncConverterAttributeFileName)!,
                                                                                          Encoding.UTF8)));

            IncrementalValuesProvider<MethodDeclarationSyntax> methodDeclarations = context.SyntaxProvider
                                                                                         .CreateSyntaxProvider(predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                                                                                                               transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                                                                                         .Where(static m => m is not null)!;

            IncrementalValuesProvider<(MethodDeclarationSyntax ClassDef, Compilation Compilation)> compilationAndClasses
                = methodDeclarations.Combine(context.CompilationProvider);

            context.RegisterSourceOutput(compilationAndClasses,
                                         static (spc, source) => Execute(source.Compilation, source.ClassDef, spc));
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        {
            return node is MethodDeclarationSyntax mds
                   && mds.AttributeLists.Count > 0
                   && !String.Equals(mds.Identifier.ValueText, "AsyncToSyncConverterAttribute", StringComparison.Ordinal);
        }

        private static MethodDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            // we know the node is a MethodDeclarationSyntax thanks to IsSyntaxTargetForGeneration
            var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;

            // loop through all the attributes on the method
            foreach (AttributeListSyntax attributeListSyntax in methodDeclarationSyntax.AttributeLists)
            {
                foreach (var (_, attributeSymbol) in FilterAttributes(attributeListSyntax))
                {
                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string fullName = attributeContainingTypeSymbol.ToDisplayString();

                    // Is the attribute the [ExceptionGenerator] attribute?
                    if (fullName == AsyncToSyncConverterAttributeFqn)
                    {
                        // return the method
                        return methodDeclarationSyntax;
                    }
                }
            }

            // we didn't find the attribute we were looking for
            return null;

            // moving the non testable code (the continue) to its own method to ignore it
            [ExcludeFromCodeCoverage]
            List<(AttributeSyntax, IMethodSymbol)> FilterAttributes(AttributeListSyntax attributeListSyntax)
            {
                var results = new List<(AttributeSyntax, IMethodSymbol)>();

                foreach (var attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    {
                        // weird, we couldn't get the symbol, ignore it
                        continue;
                    }

                    results.Add((attributeSyntax, attributeSymbol));
                }

                return results;
            }
        }

        private static void Execute(Compilation compilation, MethodDeclarationSyntax methodDeclarationSyntax, SourceProductionContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var node = methodDeclarationSyntax;

            var remover = new AsyncToSyncConverter();

            var transformResult = remover.Transform(context,
                                                    compilation,
                                                    node);

            if (!transformResult.Success)
            {
                return;
            }

            node = transformResult.Node.NormalizeWhitespace();

            var sb = new IndentedStringBuilder();

            var className = methodDeclarationSyntax.FirstAncestorOrSelf<TypeDeclarationSyntax>(x => x.IsKind(SyntaxKind.ClassDeclaration) || x.IsKind(SyntaxKind.InterfaceDeclaration))!.Identifier.ValueText;
            var syncMethodName = GenerationHelpers.RemoveLastWord(methodDeclarationSyntax.Identifier.ValueText, "Async");

            var fileNamePrefix = className + "_" + syncMethodName;

            var result = GenericClassBuilder.GenerateClass(sb,
                                                           transformResult.NameSpace!,
                                                           transformResult.TypeDeclaration!,
                                                           fileNamePrefix,
                                                           (isb) =>
                                                           {
                                                               isb.IncreaseIndent();

                                                               isb.Add(GenerationHelpers.BuildTypeDefinition(transformResult.TypeDeclaration!), addNewLine: true);
                                                               isb.Add("{", addNewLine: true);

                                                               var sm = compilation.GetSemanticModel(methodDeclarationSyntax.SyntaxTree);

                                                               isb.IncreaseIndent();
                                                               //isb.Add(node.ToFullString(),
                                                               //        addNewLine: true);

                                                               // xml comments

                                                               //// method 'header'

                                                               //foreach (var modifier in methodDeclarationSyntax.Modifiers)
                                                               //{
                                                               //    if (!modifier.IsKind(SyntaxKind.AsyncKeyword))
                                                               //    {
                                                               //        isb.Add(modifier.ValueText);
                                                               //    }
                                                               //}

                                                               var sb = new StringBuilder(); // todo use isb

                                                               ProcessNode(compilation,
                                                                           sm,
                                                                           sb,
                                                                           methodDeclarationSyntax);
                                                               var indent = isb.Indent;

                                                               for (var i = 0; i < indent; i++)
                                                               {
                                                                   isb.DecreaseIndent();
                                                               }

                                                               isb.Add(sb.ToString(), addNewLine: true);


                                                               for (var i = 0; i < indent; i++)
                                                               {
                                                                   isb.IncreaseIndent();
                                                               }

                                                               //isb.Add(" ");

                                                               //var rt = sm.GetSymbolInfo(methodDeclarationSyntax.ReturnType).Symbol;

                                                               //isb.Add(GetReturnsTypeDetails(rt!, compilation));

                                                               //var rewriter = new PotatoRewriter(compilation, sm);
                                                               //var result = rewriter.Visit(methodDeclarationSyntax.SyntaxTree.GetRoot());

                                                               // methodDeclarationSyntax

                                                               // 'body'

                                                               isb.DecreaseIndent();

                                                               isb.Add("}", addNewLine: true);
                                                               isb.DecreaseIndent();
                                                           },
                                                           preAction: isb =>
                                                           {
                                                               var cuSyntaxes = methodDeclarationSyntax.Ancestors().Where(x => x.IsKind(SyntaxKind.CompilationUnit));
                                                               var hasUsings = false;

                                                               foreach (var cuSyntax in cuSyntaxes!.Cast<CompilationUnitSyntax>())
                                                               {
                                                                   foreach (var usingDirectiveSyntax in cuSyntax.Usings)
                                                                   {
                                                                       hasUsings = true;
                                                                       isb.Add(usingDirectiveSyntax.ToString(),
                                                                                   addNewLine: true);
                                                                   }
                                                               }

                                                               if (hasUsings)
                                                               {
                                                                   isb.Add(String.Empty,
                                                                               addNewLine: true);
                                                               }
                                                           });

            var content = sb.ToString();

            context.AddSource(result.FileName!,
                              SourceText.From(content,
                                              Encoding.UTF8));
        }

        private struct Symbolizer
        {
            private readonly SemanticModel _sm;
            private readonly SyntaxNode _node;
            private SymbolInfo? symbol = null;

            public Symbolizer(SemanticModel sm, SyntaxNode node)
            {
                _sm = sm;
                _node = node;
            }

            public SymbolInfo SymbolInfo
            {
                get
                {
                    return symbol ??= _sm.GetSymbolInfo(_node);
                }
            }
        }

        private static void ProcessNode(Compilation compilation, SemanticModel sm, StringBuilder sb, SyntaxNode node)
        {
            if (node.ToFullString().Contains("ReturnGenericTask"))
            {
            }

            bool processChildNodes = true;
            var symbolizer = new Symbolizer(sm, node);

            if (node.IsKind(SyntaxKind.Block))
            {
                //processChildNodes = true;
            }
            else if (node.IsKind(SyntaxKind.IdentifierName)
                     && node.Parent is InvocationExpressionSyntax
                     && node is IdentifierNameSyntax ins)
            {
                sb.AppendWithTrivia(RemoveAsyncFrom(ins).WithTriviaFrom(ins));

                processChildNodes = false;
            }
            else if (node.IsKind(SyntaxKind.IdentifierName)
                     && node.Parent is ArgumentSyntax
                     && node is IdentifierNameSyntax argSyntax)
            {
                sb.AppendWithTrivia(RemoveAsyncFrom(argSyntax).WithTriviaFrom(argSyntax));

                processChildNodes = false;
            }
            else if (node.IsKind(SyntaxKind.IdentifierName)
                     && node.Parent is LocalFunctionStatementSyntax
                     && node is IdentifierNameSyntax argSyntax2)
            {
                if (symbolizer.SymbolInfo.Symbol is ITypeSymbol ts)
                {
                    ProcessReturnStatementSyntax(compilation,
                                                 sm,
                                                 sb,
                                                 node,
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
                    ProcessReturnStatementSyntax(compilation,
                                                 sm,
                                                 sb,
                                                 node,
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
                    ProcessReturnStatementSyntax(compilation,
                                                 sm,
                                                 sb,
                                                 node,
                                                 ts);
                    processChildNodes = false;
                }
            }
            else if (node.IsKind(SyntaxKind.ReturnStatement)
                     && node is ReturnStatementSyntax rss
                     && rss.Expression != null)
            {
                bool removeReturn = false;
                foreach (var ancestor in rss.Ancestors())
                {
                    if (ancestor is LocalFunctionStatementSyntax lfss)
                    {
                        if (lfss.ReturnType != null)
                        {
                            var s = sm.GetSymbolInfo(lfss.ReturnType);

                            if (IsTaskOrValueTask(s.Symbol))
                            {
                                var argType = lfss.ReturnType.DescendantNodes(x => !x.IsKind(SyntaxKind.TypeArgumentList)).OfType<TypeArgumentListSyntax>().FirstOrDefault();

                                removeReturn = argType == null || argType.Arguments.Count == 0;
                            }
                        }

                        break;
                    }
                    else if (ancestor is MethodDeclarationSyntax mds)
                    {
                        if (mds.ReturnType != null)
                        {
                            var s = sm.GetSymbolInfo(mds.ReturnType);

                            if (IsTaskOrValueTask(s.Symbol))
                            {
                                var argType = mds.ReturnType.DescendantNodes(x => !x.IsKind(SyntaxKind.TypeArgumentList)).OfType<TypeArgumentListSyntax>().FirstOrDefault();

                                removeReturn = argType == null || argType.Arguments.Count == 0;
                            }
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
                                sb.AddLeadingTrivia(token);
                            }
                            else
                            {
                                ProcessToken(sb, token);
                            }
                        }
                        else
                        {
                            ProcessNode(compilation, sm, sb, childNodesAndToken.AsNode()!);
                        }
                    }

                    processChildNodes = false;
                }
            }
            else if (node.IsKind(SyntaxKind.AttributeList))
            {
                processChildNodes = false;
                ProcesAttributeList(sb, sm, node);
            }
            else if (IsCancellationTokenRelated(compilation, sm, node))
            {
                processChildNodes = false;
            }
            else if (node.IsKind(SyntaxKind.LocalFunctionStatement)
                     && node is LocalFunctionStatementSyntax lfss)
            {
                //sb.AddLeadingTrivia(lfss);
            }
            else if (node.IsKind(SyntaxKind.ForEachStatement)
                     && node is ForEachStatementSyntax forEachStatement
                     && forEachStatement.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword))
            {
                processChildNodes = false;
               
                foreach (var child in node.ChildNodesAndTokens())
                {
                    if (child.IsKind(SyntaxKind.AwaitKeyword))
                    {
                        sb.AddLeadingTrivia(node);
                        continue;
                    }

                    if (child.IsToken)
                    {
                        ProcessToken(sb, child.AsToken()!);
                    }
                    else
                    {
                        ProcessNode(compilation, sm, sb, child.AsNode()!);
                    }
                }
            }
            else if (node.IsKind(SyntaxKind.AwaitExpression))
            {
                var unfoldedExpr = UnfoldAwaitExpression(compilation, sm, node);

                if (TryTaskDelay(compilation, sm, unfoldedExpr!, out var argList))
                {
                    var nnn = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                   SyntaxFactory.ParseExpression("System.Threading.Thread"),
                                                                   SyntaxFactory.IdentifierName(nameof(System.Threading.Thread.Sleep)));

                    var inv4 = SyntaxFactory.InvocationExpression(nnn,
                                                                  argList!);

                    inv4 = inv4.WithTriviaFrom(node!);

                    sb.Append(inv4.ToFullString());
                }
                else
                {
                    // todo: add test with 0 param, with 2 async params, with 2 async param and 1 non async, with async inside async

                    if (unfoldedExpr is InvocationExpressionSyntax ies7
                        && ies7.ArgumentList.Arguments.Count > 0
                        && ies7.Expression is IdentifierNameSyntax ins7)
                    {
                        var newArgList = ies7.ArgumentList;
                        var tempSb = new StringBuilder();

                        for (int i = 0, cpt = 0; i < ies7.ArgumentList.Arguments.Count; i++)
                        {
                            var arg = ies7.ArgumentList.Arguments[i];

                            if (IsCancellationTokenRelated(compilation, sm, arg))
                            {
                                newArgList = newArgList.WithArguments(newArgList.Arguments.RemoveAt(cpt));
                            }
                            else if (arg.Expression is LiteralExpressionSyntax)
                            {
                                cpt++;
                            }
                            else
                            {
                                ProcessNode(compilation, sm, tempSb, arg);

                                var newNode2 = CSharpSyntaxTree.ParseText(tempSb.ToString());

                                SyntaxNode ee = newNode2.GetRoot();

                                ExpressionSyntax? newArgExpr = null;
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
                                else
                                {
                                    // something went wrong somehow, leave thing as is?
                                }

                                tempSb.Clear();

                                cpt++;
                            }
                        }

                        var newIdentifier = RemoveAsyncFrom(ins7);

                        var newNode = ies7.WithArgumentList(newArgList.WithTriviaFrom(ies7.ArgumentList))
                                         .WithExpression(newIdentifier)
                                         .WithTriviaFrom(node);

                        sb.AppendWithTrivia(newNode);
                    }
                    else if (unfoldedExpr is InvocationExpressionSyntax ies8
                             && ies8.Expression is IdentifierNameSyntax ins8)
                    {
                        var newIdentifier = RemoveAsyncFrom(ins8);

                        var newNode = ies8.WithExpression(newIdentifier)
                                          .WithTriviaFrom(node);

                        sb.AppendWithTrivia(newNode);
                    }
                    else
                    {
                        sb.AppendWithTrivia(unfoldedExpr.WithTriviaFrom(node));
                    }
                }

                processChildNodes = false;
            }
            else if (node.IsKind(SyntaxKind.ParameterList)
                     && node is ParameterListSyntax pls)
            {
                processChildNodes = false;

                if (pls.Parameters.Count == 0)
                {
                    sb.AppendWithTrivia(pls);
                }
                else
                {
                    //var newList = new SeparatedSyntaxList<ParameterSyntax>();

                    //int cpt = 0;
                    int cpt = pls.Parameters.Count - 1;

                    var newPls = pls;

                    while (cpt >= 0)
                    {
                        var ps = pls.Parameters[cpt];
                        var newPs = ProcessParameterSyntax(compilation, sm, ps);

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


                    //foreach (var ps in pls.Parameters)
                    //{
                    //    var newPs = ProcessParameterSyntax(compilation, sm, ps);

                    //    if (newPs != null)
                    //    {
                    //        newList = newList.Add(newPs.WithTriviaFrom(pls.Parameters[cpt]));
                    //        cpt++;
                    //    }
                    //}

                    //var newPls = pls.WithParameters(newList);

                    sb.AppendWithTrivia(newPls);
                }
            }
            else if (node.IsKind(SyntaxKind.Parameter)
                     && node is ParameterSyntax ps
                     && ps.Type != null)
            {
                sb.AddLeadingTrivia(node);
                
                var newPs = ProcessParameterSyntax(compilation, sm, ps);

                sb.Append(newPs);

                sb.AddTrailingTrivia(node);

                processChildNodes = false;
            }

            if (processChildNodes)
            {
                foreach (var childNodesAndToken in node.ChildNodesAndTokens())
                {
                    if (childNodesAndToken.IsToken)
                    {
                        ProcessToken(sb, childNodesAndToken.AsToken()!);
                    }
                    else
                    {
                        ProcessNode(compilation, sm, sb, childNodesAndToken.AsNode()!);
                    }
                }
            }
        }

        private static ParameterSyntax? ProcessParameterSyntax(Compilation compilation, SemanticModel sm, ParameterSyntax parameterSyntax)
        {
            var symbol4 = sm.GetSymbolInfo(parameterSyntax.Type!);
            var processedParameter = parameterSyntax.WithIdentifier(RemoveAsyncFrom(parameterSyntax.Identifier));

            if (symbol4.Symbol != null)
            {
                if (symbol4.Symbol.Equals(compilation.GetTypeByMetadataName("System.Threading.CancellationToken"),
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
                    //string? newText = nchild.AsNode()!.ToString();

                    if (argType != null)
                    {
                        if (argType.Arguments.Count > 0)
                        {
                            var lastArg = argType.Arguments.Last();
                            var lastArgSymbol = sm.GetSymbolInfo(lastArg).Symbol;

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
                                        var newArgs2 = RemoveCancellationToken(compilation, sm, argType);
                                        var newArgs = newArgs2.Arguments.RemoveAt(newArgs2.Arguments.Count - 1);
                                        
                                        var actParam = SyntaxFactory.GenericName(SyntaxFactory.Identifier("System.Action"),
                                                                                 SyntaxFactory.TypeArgumentList(newArgs))
                                                                    .WithTriviaFrom(parameterSyntax.Type);

                                        newParam = newParam.WithType(actParam);
                                    }
                                }
                                else // Task<>
                                {
                                    var newArgs2 = RemoveCancellationToken(compilation, sm, argType);
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

        private static void ProcessReturnStatementSyntax(Compilation compilation, SemanticModel sm, StringBuilder sb, SyntaxNode originalNode, ITypeSymbol rs)
        {
            var rtd = GetReturnsTypeDetails(rs, compilation);

            if (originalNode!.HasLeadingTrivia)
            {
                sb.Append(originalNode.GetLeadingTrivia().ToFullString());
            }

            sb.Append(rtd);

            if (originalNode!.HasTrailingTrivia)
            {
                sb.Append(originalNode.GetTrailingTrivia().ToFullString());
            }
        }

        private static bool TryTaskDelay(Compilation compilation, SemanticModel sm, SyntaxNode syntaxNode, out ArgumentListSyntax? argList)
        {
            if (syntaxNode.IsKind(SyntaxKind.AwaitExpression)
                && syntaxNode is AwaitExpressionSyntax awaitSyntax)
            {
                if (awaitSyntax!.Expression is InvocationExpressionSyntax ies2)
                {
                    InvocationExpressionSyntax expr = ies2;

                    if (TryConfigureAwaitRelated(compilation,
                                                 sm,
                                                 ies2,
                                                 out var mae2))
                    {
                        if (mae2!.Expression is InvocationExpressionSyntax ies3)
                        {
                            expr = ies3;
                        }
                    }

                    var maeSymbol = sm.GetSymbolInfo(expr).Symbol;

                    if (maeSymbol != null
                        && maeSymbol.ContainingType != null
                        && maeSymbol.ContainingType.ToString() == "System.Threading.Tasks.Task"
                        && maeSymbol.Name == nameof(System.Threading.Tasks.Task.Delay))
                    {
                        argList = RemoveCancellationToken(compilation,
                                                          sm,
                                                          expr.ArgumentList).WithTriviaFrom(expr.ArgumentList);

                        return true;
                    }

                    //if (ies2.Expression is MemberAccessExpressionSyntax mae)
                    //{
                    //    var maeSymbol = sm.GetSymbolInfo(mae).Symbol;

                    //    if (maeSymbol != null
                    //        && maeSymbol.ContainingType != null
                    //        && maeSymbol.ContainingType.ToString() == "System.Threading.Tasks.Task"
                    //        && maeSymbol.Name == nameof(System.Threading.Tasks.Task.Delay))
                    //    {
                    //        return true;
                    //    }
                    //}
                }
            }
            else if (syntaxNode.IsKind(SyntaxKind.InvocationExpression)
                     && syntaxNode is InvocationExpressionSyntax ies)
            {
                var iesSymbol = sm.GetSymbolInfo(ies).Symbol;
                if (iesSymbol != null
                    && iesSymbol.ContainingType != null
                    && iesSymbol.ContainingType.ToString() == "System.Threading.Tasks.Task"
                    && iesSymbol.Name == nameof(System.Threading.Tasks.Task.Delay))
                {
                    argList = RemoveCancellationToken(compilation,
                                                      sm,
                                                      ies.ArgumentList).WithTriviaFrom(ies.ArgumentList);

                    return true;
                }
            }

            argList = null;

            return false;
        }

        private static ArgumentListSyntax RemoveCancellationToken(Compilation compilation, SemanticModel sm, ArgumentListSyntax argList)
        {
            if (argList.Arguments.Count == 0)
            {
                return argList;
            }

            var newArgList = argList;

            int cpt = argList.Arguments.Count - 1;

            while (cpt >= 0)
            {
                var arg = argList.Arguments[cpt];

                var ti = sm.GetTypeInfo(arg.Expression);

                if (ti.ConvertedType != null
                    && ti.ConvertedType!.Equals(compilation.GetTypeByMetadataName("System.Threading.CancellationToken"),
                                                SymbolEqualityComparer.Default))
                {
                    newArgList = newArgList.WithArguments(newArgList.Arguments.RemoveAt(cpt));
                }

                cpt--;
            }

            return newArgList;
        }

        private static TypeArgumentListSyntax RemoveCancellationToken(Compilation compilation, SemanticModel sm, TypeArgumentListSyntax argList)
        {
            if (argList.Arguments.Count == 0)
            {
                return argList;
            }

            var newArgList = argList;

            int cpt = argList.Arguments.Count - 1;

            while (cpt >= 0)
            {
                var arg = argList.Arguments[cpt];

                var ti = sm.GetTypeInfo(arg);

                if (ti.ConvertedType != null
                    && ti.ConvertedType!.Equals(compilation.GetTypeByMetadataName("System.Threading.CancellationToken"),
                                                SymbolEqualityComparer.Default))
                {
                    newArgList = newArgList.WithArguments(newArgList.Arguments.RemoveAt(cpt));
                }

                cpt--;
            }

            return newArgList;
        }

        private static SyntaxNode UnfoldAwaitExpression(Compilation compilation, SemanticModel sm, SyntaxNode node)
        {
            if (node.IsKind(SyntaxKind.AwaitExpression))
            {
                var awaitSyntax = node as AwaitExpressionSyntax;
                node = awaitSyntax!;

                if (TryConfigureAwaitRelated(compilation, sm, awaitSyntax!.Expression, out var maes6))
                {
                    node = maes6!.Expression;
                }
                else
                {
                    node = awaitSyntax.Expression;
                }
            }

            return node;
        }

        private static bool TryConfigureAwaitRelated(Compilation compilation, SemanticModel sm, SyntaxNode node, out MemberAccessExpressionSyntax? maesX)
        {
            if (node.IsKind(SyntaxKind.InvocationExpression))
            {
                var ies = node as InvocationExpressionSyntax;

                if (ies!.Expression is MemberAccessExpressionSyntax memberAccess
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
                        maesX = memberAccess;
                        return true;
                    }

                    maesX = memberAccess;
                    return true;
                }
            }

            if (node.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var smae = node as MemberAccessExpressionSyntax;

                if ((
                        string.Equals(smae!.Name.Identifier.ValueText,
                                      "ConfigureAwait",
                                      StringComparison.Ordinal)
                        || string.Equals(smae.Name.Identifier.ValueText,
                                         "ConfigureAwaitWithCulture",
                                         StringComparison.Ordinal)
                    )
                    && smae.Expression is InvocationExpressionSyntax inv)
                {
                    if (inv.Expression is IdentifierNameSyntax identifierNameSyntax)
                    {
                        maesX = smae;
                        return true;
                    }

                    maesX = smae;
                    return true;
                }
            }

            maesX = null;
            return false;
        }

        private static void ProcessToken(StringBuilder sb, SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.AsyncKeyword)
                || token.IsKind(SyntaxKind.AwaitKeyword))
            {
                sb.AddLeadingTrivia(token);
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
                    sb.AddLeadingTrivia(token);
                    sb.Append(RemoveAsyncFrom(token));
                    sb.AddTrailingTrivia(token);
                }
                else
                {
                    sb.Append(token.ToFullString());
                }
            }
        }
    }
}