using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace NoWoL.SourceGenerators
{
    [Generator]
    public class AlwaysInitializedPropertyGenerator : IIncrementalGenerator
    {
        private const string AlwaysInitializedPropertyAttributeFqn = "NoWoL.SourceGenerators.AlwaysInitializedPropertyAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource("AlwaysInitializedPropertyAttribute.g.cs",
                                                                          SourceText.From(EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoader).Assembly,
                                                                                                                     EmbeddedResourceLoader.AlwaysInitializedPropertyAttributeFileName)!,
                                                                                          Encoding.UTF8)));

            // Gather info for all annotated fields
            IncrementalValuesProvider<(FieldDeclarationSyntax FieldSyntax, DiagnosticDescriptor? Diagnostic)> fieldsWithError
                = context.SyntaxProvider.ForAttributeWithMetadataName(AlwaysInitializedPropertyAttributeFqn,
                                                                      static (s, token) => IsSyntaxTargetForGeneration(s, token),
                                                                      static (ctx, token) => GetSemanticTargetForGeneration(ctx, token))
                         .Where(static m => m.FieldSyntax is not null)!;

            // Output the diagnostics
            context.RegisterSourceOutput(fieldsWithError.Where(x => x.Diagnostic is not null),
                                         static (context, fwd) =>
                                         {
                                             var firstVariable = fwd.FieldSyntax.Declaration.Variables.First();
                                             var error = Diagnostic.Create(fwd.Diagnostic!,
                                                                           firstVariable.GetLocation(),
                                                                           firstVariable.Identifier.Text);

                                             context.ReportDiagnostic(error);
                                         });

            // Get the filtered sequence to enable caching
            IncrementalValuesProvider<(ClassDeclarationSyntax Parent, FieldDeclarationSyntax FieldSyntax)> classWithField 
                = fieldsWithError.Where(static item => item.Diagnostic is null).Select((x, _) => ((ClassDeclarationSyntax)x.FieldSyntax.Parent!, x.FieldSyntax));

            IncrementalValuesProvider<(ClassDeclarationSyntax Key, ImmutableArray<FieldDeclarationSyntax> Fields)> groupedFields
                = classWithField.Collect().SelectMany((item, token) =>
                                                  {
                                                      Dictionary<ClassDeclarationSyntax, ImmutableArray<FieldDeclarationSyntax>.Builder> map = new();

                                                      foreach ((ClassDeclarationSyntax, FieldDeclarationSyntax) pair in item)
                                                      {
                                                          ClassDeclarationSyntax key = pair.Item1;
                                                          FieldDeclarationSyntax element = pair.Item2;

                                                          if (!map.TryGetValue(key,
                                                                               out ImmutableArray<FieldDeclarationSyntax>.Builder builder))
                                                          {
                                                              builder = ImmutableArray.CreateBuilder<FieldDeclarationSyntax>();

                                                              map.Add(key,
                                                                      builder);
                                                          }

                                                          builder.Add(element);
                                                      }

                                                      token.ThrowIfCancellationRequested();

                                                      ImmutableArray<(ClassDeclarationSyntax Key, ImmutableArray<FieldDeclarationSyntax> Fields)>.Builder result =
                                                          ImmutableArray.CreateBuilder<(ClassDeclarationSyntax, ImmutableArray<FieldDeclarationSyntax>)>();

                                                      foreach (KeyValuePair<ClassDeclarationSyntax, ImmutableArray<FieldDeclarationSyntax>.Builder> entry in map)
                                                      {
                                                          result.Add((entry.Key, entry.Value.ToImmutable()));
                                                      }

                                                      return result;
                                                  });

            // Generate the requested properties and methods
            context.RegisterSourceOutput(groupedFields, static (context, item) =>
                                                        {
                                                            var result = GeneratePartialClass(context,
                                                                                              item.Key,
                                                                                              item.Fields);

                                                            context.AddSource(result.FileName,
                                                                              result.Content);
                                                        });
        }

        private static (string FileName, SourceText Content) GeneratePartialClass(SourceProductionContext context, 
                                                                                  ClassDeclarationSyntax classDeclarationSyntax, 
                                                                                  ImmutableArray<FieldDeclarationSyntax> fields)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var ns = GenerationHelpers.GetNamespace(classDeclarationSyntax);

            var classToGenerate = new AlwaysInitializedPropertyClassToGenerate(classDeclarationSyntax,
                                                                               fields,
                                                                               ns);

            var sb = new IndentedStringBuilder();
            var classBuilder = new AlwaysInitializedPropertyClassBuilder();
            var result = classBuilder.Generate(sb, classToGenerate);

            return (result.FileName!,
                    SourceText.From(sb.ToString(),
                                    Encoding.UTF8));
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return syntaxNode.IsKind(SyntaxKind.VariableDeclarator)
                   && syntaxNode.Parent is VariableDeclarationSyntax { Parent: FieldDeclarationSyntax };
        }

        private static (FieldDeclarationSyntax? FieldSyntax, DiagnosticDescriptor? Diagnostic) GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext ctx, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var fieldSyntax = (FieldDeclarationSyntax)ctx.TargetNode.Parent!.Parent!;

            if (!TryValidateTarget(ctx,
                                   fieldSyntax,
                                   out var diag,
                                   out var ns))
            {
                return (fieldSyntax, diag);
            }

            return (fieldSyntax, null);
        }

        internal static bool TryValidateTarget(GeneratorAttributeSyntaxContext context, FieldDeclarationSyntax target, out DiagnosticDescriptor? diagnosticToReport, out string? ns)
        {
            if (target.Parent is not ClassDeclarationSyntax)
            {
                diagnosticToReport = AlwaysInitializedPropertyGeneratorDescriptors.FieldMustBeInClass;
                ns = null;

                return false;
            }

            var parentClasses = target.Ancestors().Where(x => x.IsKind(SyntaxKind.ClassDeclaration)).OfType<ClassDeclarationSyntax>();
            if (parentClasses.Any(x => !GenerationHelpers.IsPartialType(x)))
            {
                diagnosticToReport = AlwaysInitializedPropertyGeneratorDescriptors.MustBeInParentPartialClass;
                ns = null;

                return false;
            }

            if (!target.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            {
                diagnosticToReport = AlwaysInitializedPropertyGeneratorDescriptors.FieldMustBePrivate;
                ns = null;

                return false;
            }

            if (target.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
            {
                diagnosticToReport = AlwaysInitializedPropertyGeneratorDescriptors.FieldCannotBeStatic;
                ns = null;

                return false;
            }

            if (target.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword)))
            {
                diagnosticToReport = AlwaysInitializedPropertyGeneratorDescriptors.FieldCannotBeReadOnly;
                ns = null;

                return false;
            }

            if (target.Declaration.Variables.Count != 1)
            {
                diagnosticToReport = AlwaysInitializedPropertyGeneratorDescriptors.OnlyOneFieldCanBeDeclared;
                ns = null;

                return false;
            }

            var parentClass = (ClassDeclarationSyntax)target.Parent;

            ns = GenerationHelpers.GetNamespace(parentClass);

            if (String.IsNullOrWhiteSpace(ns))
            {
                diagnosticToReport = AlwaysInitializedPropertyGeneratorDescriptors.FieldMustBeInNamespace;

                return false;
            }

            var sm = context.SemanticModel;
            var typeSymbol = sm.GetTypeInfo(target.Declaration.Type);

            if (typeSymbol.Type is IErrorTypeSymbol)
            {
                diagnosticToReport = AlwaysInitializedPropertyGeneratorDescriptors.FieldTypeMustExist;

                return false;
            }

            var stringSymbol = sm.Compilation.GetTypeByMetadataName("System.String");

            if (typeSymbol.Type is { IsValueType: true }
                || typeSymbol.Type!.Equals(stringSymbol, SymbolEqualityComparer.Default))
            {
                diagnosticToReport = AlwaysInitializedPropertyGeneratorDescriptors.FieldTypeMustBeAReferenceType;

                return false;
            }

            var members = typeSymbol.Type!.GetMembers(".ctor");

            if (!members.Any(x => x is IMethodSymbol ms && ms.Parameters.IsEmpty))
            {
                diagnosticToReport = AlwaysInitializedPropertyGeneratorDescriptors.FieldTypeMustHaveParameterlessConstructor;

                return false;
            }

            diagnosticToReport = null;

            return true;
        }
    }
}
