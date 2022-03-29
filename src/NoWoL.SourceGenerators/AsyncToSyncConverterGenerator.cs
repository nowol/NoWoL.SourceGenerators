using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;

namespace NoWoL.SourceGenerators
{
    [Generator]
    public class AsyncToSyncConverterGenerator : IIncrementalGenerator
    {
        // This class was adapted from NetEscapades.EnumGenerators
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

            IncrementalValueProvider<(Compilation, ImmutableArray<MethodDeclarationSyntax>)> compilationAndMethods
                = context.CompilationProvider.Combine(methodDeclarations.Collect());

            context.RegisterSourceOutput(compilationAndMethods,
                                         static (spc, source) => Execute(source.Item1, source.Item2, spc));
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

        private static void Execute(Compilation compilation, ImmutableArray<MethodDeclarationSyntax> methods, SourceProductionContext context)
        {
            if (!CanExecute(methods, compilation))
            {
                return;
            }

            IEnumerable<MethodDeclarationSyntax> distinctMethods = methods.Distinct();

            foreach (var methodDeclarationSyntax in distinctMethods)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var node = methodDeclarationSyntax;

                var remover = new AsyncToSyncConverter();

                var transformResult = remover.Transform(context,
                                                        compilation,
                                                        node);

                if (!transformResult.Success)
                {
                    continue;
                }

                node = transformResult.Node.NormalizeWhitespace();

                var sb = new IndentedStringBuilder();

                var className = (methodDeclarationSyntax.FirstAncestorOrSelf<SyntaxNode>(x => x.IsKind(SyntaxKind.ClassDeclaration)) as ClassDeclarationSyntax)!.Identifier.ValueText;
                var syncMethodName = GenerationHelpers.RemoveLastWord(methodDeclarationSyntax.Identifier.ValueText, "Async");

                var fileNamePrefix = className + "_" + syncMethodName;

                var result = GenericClassBuilder.GenerateClass(sb,
                                                               transformResult.NameSpace!,
                                                               transformResult.ClassDeclaration!,
                                                               fileNamePrefix,
                                                               (isb) =>
                                                               {
                                                                   isb.IncreaseIndent();

                                                                   isb.Add(GenerationHelpers.BuildClassDefinition(transformResult.ClassDeclaration!), addNewLine: true);
                                                                   isb.Add("{", addNewLine: true);

                                                                   isb.IncreaseIndent();
                                                                   isb.Add(node.ToFullString(),
                                                                           addNewLine: true);
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
        }

        [ExcludeFromCodeCoverage]
        private static bool CanExecute(ImmutableArray<MethodDeclarationSyntax> methods, Compilation compilation)
        {
            if (methods.IsDefaultOrEmpty)
            {
                return false;
            }

            var methodAttribute = compilation.GetTypeByMetadataName(AsyncToSyncConverterAttributeFqn);
            if (methodAttribute == null)
            {
                // nothing to do if this type isn't available
                return false;
            }

            return true;
        }
    }
}