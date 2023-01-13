using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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

            IncrementalValuesProvider<MethodDeclarationSyntax> methodDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(AsyncToSyncConverterAttributeFqn,
                                                                                                                                        static (s, token) => IsSyntaxTargetForGeneration(s, token),
                                                                                                                                        static (ctx, token) => GetSemanticTargetForGeneration(ctx, token))
                                                                                           .Where(static m => m is not null)!;

            IncrementalValuesProvider<(MethodDeclarationSyntax ClassDef, Compilation Compilation)> compilationAndClasses
                = methodDeclarations.Combine(context.CompilationProvider);

            context.RegisterSourceOutput(compilationAndClasses,
                                         static (spc, source) => Execute(source.Compilation, source.ClassDef, spc));
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return node.IsKind(SyntaxKind.MethodDeclaration);
        }

        private static MethodDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // we know the node is a MethodDeclarationSyntax thanks to IsSyntaxTargetForGeneration
            var methodDeclarationSyntax = (MethodDeclarationSyntax)context.TargetNode;

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

            var sb = new IndentedStringBuilder();
            var processor = new AsyncToSyncProcessor(compilation, context, methodDeclarationSyntax, sb);

            var ancestorAnalysisResults = processor.AnalyzeAncestors(methodDeclarationSyntax);

            if (!ancestorAnalysisResults.Result)
            {
                return;
            }

            var className = methodDeclarationSyntax.FirstAncestorOrSelf<TypeDeclarationSyntax>(x => x.IsKind(SyntaxKind.ClassDeclaration) || x.IsKind(SyntaxKind.InterfaceDeclaration))!.Identifier.ValueText;
            var syncMethodName = GenerationHelpers.RemoveLastWord(methodDeclarationSyntax.Identifier.ValueText, "Async");

            var fileNamePrefix = className + "_" + syncMethodName;

            var result = GenericClassBuilder.GenerateClass(sb,
                                                           ancestorAnalysisResults.NameSpace!,
                                                           ancestorAnalysisResults.TypeDeclaration!,
                                                           fileNamePrefix,
                                                           (isb) =>
                                                           {
                                                               isb.IncreaseIndent();

                                                               isb.Add(GenerationHelpers.BuildTypeDefinition(ancestorAnalysisResults.TypeDeclaration!), addNewLine: true);
                                                               isb.Add("{", addNewLine: true);
                                                               
                                                               processor.ProcessNode(methodDeclarationSyntax);

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

            if (!processor.ContainsDiagnosticErrors)
            {
                var content = sb.ToString();

                context.AddSource(result.FileName!,
                                  SourceText.From(content,
                                                  Encoding.UTF8));
            }
        }
    }
}