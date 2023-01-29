using System;
using System.Collections.Generic;
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
        private const string ExceptionGeneratorAttributeFqn = "NoWoL.SourceGenerators.ExceptionGeneratorAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource("ExceptionGeneratorAttributeFqn.g.cs",
                                                                          SourceText.From(EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoader).Assembly,
                                                                                                                     EmbeddedResourceLoader.ExceptionGeneratorAttributeFileName)!,
                                                                                          Encoding.UTF8)));

            IncrementalValuesProvider<ExceptionClassDefinition> allClasses = context.SyntaxProvider.ForAttributeWithMetadataName(ExceptionGeneratorAttributeFqn,
                                                                                                                                 static (s, token) => IsSyntaxTargetForGeneration(s, token),
                                                                                                                                 static (ctx, token) => GetSemanticTargetForGeneration(ctx, token));
            IncrementalValuesProvider<ExceptionClassDefinition> withErrors = allClasses.Where(static m => m.DiagnosticDef.Initialized);
            IncrementalValuesProvider<ExceptionClassDefinition> withoutErrors = allClasses.Where(static m => !m.DiagnosticDef.Initialized);

            context.RegisterSourceOutput(withErrors,
                                         (productionContext, definition) => {

                                             var error = Diagnostic.Create(definition.DiagnosticDef.Diagnostic!,
                                                                           definition.DiagnosticDef.Location,
                                                                           definition.DiagnosticDef.Parameter);

                                             productionContext.ReportDiagnostic(error);
                                         });

            context.RegisterSourceOutput(withoutErrors,
                                         static (spc, source) => Execute(source, spc));
        }

        private static void Execute(ExceptionClassDefinition classDefinition, SourceProductionContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var sb = new IndentedStringBuilder();
            var classBuilder = new ExceptionClassBuilder();
            classBuilder.GenerateException(sb, ref classDefinition);

            var filename = GenericClassBuilder.GenerateFilename(classDefinition.ClassDef.Name,
                                                                classDefinition.ClassDef,
                                                                classDefinition.Namespace!,
                                                                classDefinition.ParentClasses);

            context.AddSource(filename,
                              SourceText.From(sb.ToString(),
                                              Encoding.UTF8));

        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return node.IsKind(SyntaxKind.ClassDeclaration);
        }

        private static ExceptionClassDefinition GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var cls = (ClassDeclarationSyntax)context.TargetNode;

            var def = new ExceptionClassDefinition();

            var ns = GenerationHelpers.GetNamespace(cls);

            def.ClassDef = new ClassDefinition {
                                                   Name = cls.Identifier.ValueText,
                                                   Modifier = String.Join(" ", cls.Modifiers.Select(x => x.ValueText))
                                               };
            def.Namespace = ns;

            if (!GenerationHelpers.IsPartialType(cls))
            {
                def.SetDiagnostic(new DiagnosticDefinition
                                  {
                                      Diagnostic = ExceptionGeneratorDescriptors.MethodMustBePartial,
                                      Location = cls.Identifier.GetLocation(),
                                      Parameter = cls.Identifier.Text,
                                      Initialized = true
                                  });
            }

            if (String.IsNullOrWhiteSpace(def.Namespace))
            {
                def.SetDiagnostic(new DiagnosticDefinition
                                  {
                                      Diagnostic = ExceptionGeneratorDescriptors.MethodClassMustBeInNamespace,
                                      Location = cls.Identifier.GetLocation(),
                                      Parameter = cls.Identifier.Text,
                                      Initialized = true
                                  });
            }

            var parentClasses = cls.Ancestors().Where(x => x.IsKind(SyntaxKind.ClassDeclaration)).OfType<ClassDeclarationSyntax>().Reverse();

            foreach (var parentClass in parentClasses)
            {
                if (!GenerationHelpers.IsPartialType(parentClass))
                {
                    def.SetDiagnostic(new DiagnosticDefinition
                                      {
                                          Diagnostic = ExceptionGeneratorDescriptors.MustBeInParentPartialClass,
                                          Location = cls.Identifier.GetLocation(),
                                          Parameter = cls.Identifier.Text,
                                          Initialized = true
                                      });
                }

                def.ParentClasses ??= new List<ClassDefinition>();

                def.ParentClasses.Add(new ClassDefinition
                                      {
                                          Name = parentClass.Identifier.ValueText,
                                          Modifier = String.Join(" ",
                                                                 parentClass.Modifiers.Select(x => x.ValueText))
                                      });
            }

            var exceptionAttribute = context.SemanticModel.Compilation.GetTypeByMetadataName(ExceptionGeneratorAttributeFqn);

            var classSymbol = context.SemanticModel.GetDeclaredSymbol(cls,
                                                                      cancellationToken)!;
            var exceptionAttributes = classSymbol.GetAttributes().Where(x => exceptionAttribute!.Equals(x.AttributeClass, SymbolEqualityComparer.Default));

            foreach (var exceptionAttr in exceptionAttributes)
            {
                if (exceptionAttr.ConstructorArguments.Length == 1)
                {
                    var msg = exceptionAttr.ConstructorArguments[0].Value as string;

                    if (!String.IsNullOrWhiteSpace(msg))
                    {
                        def.Messages ??= new List<string>();

                        def.Messages.Add(msg!);
                    }
                }
            }
            
            return def;
        }
    }
}