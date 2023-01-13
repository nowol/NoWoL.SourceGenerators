using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace NoWoL.SourceGenerators
{
    [Generator]
    public class ExceptionClassGenerator : IIncrementalGenerator
    {
        // This class was inspired from NetEscapades.EnumGenerators
        // https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/

        private const string ExceptionGeneratorAttributeFqn = "NoWoL.SourceGenerators.ExceptionGeneratorAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource("ExceptionGeneratorAttributeFqn.g.cs",
                                                                          SourceText.From(EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoader).Assembly,
                                                                                                                     EmbeddedResourceLoader.ExceptionGeneratorAttributeFileName)!,
                                                                                          Encoding.UTF8)));

            IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(ExceptionGeneratorAttributeFqn,
                                                                                                                                      static (s, token) => IsSyntaxTargetForGeneration(s, token),
                                                                                                                                      static (ctx, token) => GetSemanticTargetForGeneration(ctx, token))
                                                                                         .Where(static m => m is not null)!;

            IncrementalValuesProvider<(ClassDeclarationSyntax ClassDef, Compilation Compilation)> compilationAndClasses
                = classDeclarations.Combine(context.CompilationProvider);

            context.RegisterSourceOutput(compilationAndClasses,
                                         static (spc, source) => Execute(source.Compilation, source.ClassDef, spc));
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return node.IsKind(SyntaxKind.ClassDeclaration);
        }

        private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // we know the node is a ClassDeclarationSyntax thanks to IsSyntaxTargetForGeneration
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.TargetNode;

            // loop through all the attributes on the method
            foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
            {
                foreach (var (_, attributeSymbol) in FilterAttributes(attributeListSyntax))
                {
                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string fullName = attributeContainingTypeSymbol.ToDisplayString();

                    // Is the attribute the [ExceptionGenerator] attribute?
                    if (fullName == ExceptionGeneratorAttributeFqn)
                    {
                        // return the class
                        return classDeclarationSyntax;
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

        private static void Execute(Compilation compilation, ClassDeclarationSyntax classDeclarationSyntax, SourceProductionContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var executionValidationResult = CanExecute(compilation,
                                                       classDeclarationSyntax,
                                                       context);

            if (!executionValidationResult.IsSuccess())
            {
                return;
            }

            var exceptionAttribute = executionValidationResult.ExceptionAttribute!;
            var classSymbol = executionValidationResult.ClassSymbol!;
            var ns = executionValidationResult.Ns!;

            var exceptionAttributes = classSymbol.GetAttributes().Where(x => exceptionAttribute.Equals(x.AttributeClass, SymbolEqualityComparer.Default)).ToList();

            var classToGenerate = new ClassToGenerate(classDeclarationSyntax,
                                                      classSymbol,
                                                      exceptionAttributes,
                                                      ns);

            var sb = new IndentedStringBuilder();
            var classBuilder = new ExceptionClassBuilder();
            var result = classBuilder.GenerateException(sb, classToGenerate);
            context.AddSource(result.FileName!,
                              SourceText.From(sb.ToString(),
                                              Encoding.UTF8));
        }

        private static CanExecuteValidationResult CanExecute(Compilation compilation,
                                                             ClassDeclarationSyntax classDeclarationSyntax,
                                                             SourceProductionContext context)
        {
            var ns = GenerationHelpers.GetNamespace(classDeclarationSyntax);

            if (!TryValidateTarget(classDeclarationSyntax, ns, out var diagnosticToReport))
            {
                var error = Diagnostic.Create(diagnosticToReport!,
                                              classDeclarationSyntax.Identifier.GetLocation(),
                                              classDeclarationSyntax.Identifier.Text);
                context.ReportDiagnostic(error);

                return new CanExecuteValidationResult(null, null, ns);
            }

            var exceptionAttribute = compilation.GetTypeByMetadataName(ExceptionGeneratorAttributeFqn);

            SemanticModel semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax,
                                                              context.CancellationToken);

            return new CanExecuteValidationResult(exceptionAttribute, classSymbol, ns);
        }

        internal static bool TryValidateTarget(ClassDeclarationSyntax target, string? ns, out DiagnosticDescriptor? diagnosticToReport)
        {
            if (!GenerationHelpers.IsPartialType(target))
            {
                diagnosticToReport = ExceptionGeneratorDescriptors.MethodMustBePartial;

                return false;
            }

            if (String.IsNullOrWhiteSpace(ns))
            {
                diagnosticToReport = ExceptionGeneratorDescriptors.MethodClassMustBeInNamespace;

                return false;
            }

            var parentClasses = target.Ancestors().Where(x => x.IsKind(SyntaxKind.ClassDeclaration)).OfType<ClassDeclarationSyntax>();
            if (parentClasses.Any(x => !GenerationHelpers.IsPartialType(x)))
            {
                diagnosticToReport = ExceptionGeneratorDescriptors.MustBeInParentPartialClass;

                return false;
            }

            diagnosticToReport = null;

            return true;
        }

        private readonly struct CanExecuteValidationResult
        {
            public INamedTypeSymbol? ExceptionAttribute { get; }

            public INamedTypeSymbol? ClassSymbol { get; }

            public string? Ns { get; }

            public CanExecuteValidationResult(INamedTypeSymbol? exceptionAttribute, INamedTypeSymbol? classSymbol, string? ns)
            {
                ExceptionAttribute = exceptionAttribute;
                ClassSymbol = classSymbol;
                Ns = ns;
            }

            public bool IsSuccess()
            {
                return ExceptionAttribute != null
                       && ClassSymbol != null
                       && Ns != null;
            }
        }
    }
}