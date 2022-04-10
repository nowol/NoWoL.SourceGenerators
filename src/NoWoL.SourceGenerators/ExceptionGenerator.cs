using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace NoWoL.SourceGenerators
{
    [Generator]
    public class ExceptionGenerator : IIncrementalGenerator
    {
        // This class was adapted from NetEscapades.EnumGenerators
        // https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/

        private const string ExceptionGeneratorAttributeFqn = "NoWoL.SourceGenerators.ExceptionGeneratorAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource("ExceptionGeneratorAttributeFqn.g.cs",
                                                                          SourceText.From(EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoader).Assembly, 
                                                                                                                     EmbeddedResourceLoader.ExceptionGeneratorAttributeFileName)!,
                                                                                          Encoding.UTF8)));

            IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
                                                                                         .CreateSyntaxProvider(predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                                                                                                               transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                                                                                         .Where(static m => m is not null)!;

            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClasses
                = context.CompilationProvider.Combine(classDeclarations.Collect());

            context.RegisterSourceOutput(compilationAndClasses,
                                         static (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        {
            return node is ClassDeclarationSyntax cds
                   && cds.AttributeLists.Count > 0
                   && !String.Equals(cds.Identifier.ValueText, "ExceptionGeneratorAttribute", StringComparison.Ordinal);
        }

        private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            // we know the node is a ClassDeclarationSyntax thanks to IsSyntaxTargetForGeneration
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

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

        private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
        {
            try
            {
                if (!CanExecute(classes, compilation, out var classAttribute))
                {
                    return;
                }

                IEnumerable<ClassDeclarationSyntax> distinctClasses = classes.Distinct();

                List<ClassToGenerate> classesToGenerate = GetTypesToGenerate(compilation,
                                                                             context,
                                                                             distinctClasses,
                                                                             classAttribute!,
                                                                             context.CancellationToken);
                if (classesToGenerate.Count > 0)
                {
                    var sb = new IndentedStringBuilder();
                    var classBuilder = new ExceptionClassBuilder();
                    foreach (var classToGenerate in classesToGenerate)
                    {
                        sb.Clear(true);
                        var result = classBuilder.GenerateException(sb, classToGenerate);
                        context.AddSource(result.FileName!, SourceText.From(sb.ToString(), Encoding.UTF8));
                    }
                }
            }
            catch (Exception ex) when (!GenerationHelpers.IsOperationCanceledException(ex))
            {
                context.ReportDiagnostic(Diagnostic.Create(GenerationHelpers.ConvertErrorCode(ExceptionGeneratorErrorCode.UnexpectedException),
                                                                                              "Exception generator",
                                                                                              "An exception occurred while generating exception: " + ex.Message,
                                                                                              defaultSeverity: DiagnosticSeverity.Error,
                                                                                              severity: DiagnosticSeverity.Error,
                                                                                              isEnabledByDefault: true,
                                                                                              warningLevel: 0));

            }
        }

        [ExcludeFromCodeCoverage]
        private static bool CanExecute(ImmutableArray<ClassDeclarationSyntax> classes, Compilation compilation, out INamedTypeSymbol? classAttribute)
        {
            if (classes.IsDefaultOrEmpty)
            {
                // nothing to do yet
                classAttribute = null;
                return false;
            }

            classAttribute = compilation.GetTypeByMetadataName(ExceptionGeneratorAttributeFqn);
            if (classAttribute == null)
            {
                // nothing to do if this type isn't available
                return false;
            }

            return true;
        }

        private static List<ClassToGenerate> GetTypesToGenerate(Compilation compilation,
                                                                SourceProductionContext context,
                                                                IEnumerable<ClassDeclarationSyntax> classes,
                                                                INamedTypeSymbol classAttribute,
                                                                CancellationToken ct)
        {
            var classesToGenerate = new List<ClassToGenerate>();

            foreach (var (classDeclarationSyntax, classSymbol) in FilterClasses())
            {
                // stop if we're asked to
                ct.ThrowIfCancellationRequested();

                var ns = GenerationHelpers.GetNamespace(classDeclarationSyntax);

                if (!ValidateTarget(context, classDeclarationSyntax, ns))
                {
                    continue;
                }

                // It's OK to use First() here since we have validated the presence of the attribute in a previous step
                var exceptionAttribute = classSymbol.GetAttributes().First(x => classAttribute.Equals(x.AttributeClass, SymbolEqualityComparer.Default));

                classesToGenerate.Add(new ClassToGenerate(classDeclarationSyntax,
                                                          classSymbol,
                                                          exceptionAttribute,
                                                          ns));
            }

            return classesToGenerate;

            // moving the non testable code (the continue) to its own method to ignore it
            [ExcludeFromCodeCoverage]
            List<(ClassDeclarationSyntax, INamedTypeSymbol)> FilterClasses()
            {
                var results = new List<(ClassDeclarationSyntax, INamedTypeSymbol)>();

                foreach (var classDeclarationSyntax in classes)
                {
                    SemanticModel semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
                    var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax, ct);

                    if (classSymbol == null)
                    {
                        // report diagnostic, something went wrong
                        continue;
                    }

                    results.Add((classDeclarationSyntax, classSymbol));
                }

                return results;
            }
        }
        
        private static bool ValidateTarget(SourceProductionContext context, ClassDeclarationSyntax target, string? ns)
        {
            if (!GenerationHelpers.IsPartialClass(target))
            {
                context.ReportDiagnostic(Diagnostic.Create(GenerationHelpers.ConvertErrorCode(ExceptionGeneratorErrorCode.MethodMustBePartial),
                                                           "Exception generator",
                                                           "The [ExceptionGenerator] must be applied to a partial class.",
                                                           defaultSeverity: DiagnosticSeverity.Error,
                                                           severity: DiagnosticSeverity.Error,
                                                           isEnabledByDefault: true,
                                                           warningLevel: 0,
                                                           location: target.GetLocation()));

                return false;
            }

            if (String.IsNullOrWhiteSpace(ns))
            {
                context.ReportDiagnostic(Diagnostic.Create(GenerationHelpers.ConvertErrorCode(ExceptionGeneratorErrorCode.MethodClassMustBeInNamespace),
                                                           "Exception generator",
                                                           "The [ExceptionGenerator] must be applied to a partial class contained in a namespace.",
                                                           defaultSeverity: DiagnosticSeverity.Error,
                                                           severity: DiagnosticSeverity.Error,
                                                           isEnabledByDefault: true,
                                                           warningLevel: 0,
                                                           location: target.GetLocation()));

                return false;
            }

            var parentClasses = target.Ancestors().Where(x => x.IsKind(SyntaxKind.ClassDeclaration)).OfType<ClassDeclarationSyntax>();
            if (parentClasses.Any(x => !GenerationHelpers.IsPartialClass(x)))
            {
                context.ReportDiagnostic(Diagnostic.Create(GenerationHelpers.ConvertErrorCode(ExceptionGeneratorErrorCode.MustBeInParentPartialClass),
                                                           "Exception generator",
                                                           "The [ExceptionGenerator] must be applied to a partial class nested in another partial class.",
                                                           defaultSeverity: DiagnosticSeverity.Error,
                                                           severity: DiagnosticSeverity.Error,
                                                           isEnabledByDefault: true,
                                                           warningLevel: 0,
                                                           location: target.GetLocation()));

                return false;
            }

            return true;
        }
    }
}