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
using NoWoL.SourceGenerators.Comparers;
using NoWoL.SourceGenerators.AlwaysInitializedProperty;

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

            IncrementalValuesProvider<AlwaysInitializedPropertyFieldDefinition> allFields = context.SyntaxProvider.ForAttributeWithMetadataName(AlwaysInitializedPropertyAttributeFqn,
                                                                                                                                                 static (s, token) => IsSyntaxTargetForGeneration(s, token),
                                                                                                                                                 static (ctx, token) => GetSemanticTargetForGeneration(ctx, token));

            IncrementalValuesProvider<AlwaysInitializedPropertyFieldDefinition> withErrors = allFields.Where(static m => m.DiagnosticDef.Initialized);
            IncrementalValuesProvider<AlwaysInitializedPropertyFieldDefinition> withoutErrors = allFields.Where(static m => !m.DiagnosticDef.Initialized);

            context.RegisterSourceOutput(withErrors,
                                         (productionContext, definition) => {

                                             var error = Diagnostic.Create(definition.DiagnosticDef.Diagnostic!,
                                                                           definition.DiagnosticDef.Location,
                                                                           definition.DiagnosticDef.Parameter);

                                             productionContext.ReportDiagnostic(error);
                                         });
            
            IncrementalValuesProvider<AlwaysInitializedPropertyClassDefinition> groupedFields
                = withoutErrors.Collect().SelectMany((item, token) =>
                                                     {
                                                         // adapted from https://github.com/CommunityToolkit/dotnet/blob/e8969781afe537ea41a964a15b4ccfee32e095df/src/CommunityToolkit.Mvvm.SourceGenerators/ComponentModel/ObservablePropertyGenerator.cs
                                                         Dictionary<ClassWithParentAndNamespaceDefinition, ImmutableArray<AlwaysInitializedPropertyFieldDefinition>.Builder> map = new();

                                                         foreach (var field in item)
                                                         {
                                                             ClassWithParentAndNamespaceDefinition key = new ClassWithParentAndNamespaceDefinition
                                                                                                         {
                                                                                                             ClassDef = field.ClassDef,
                                                                                                             Namespace = field.Namespace,
                                                                                                             ParentClasses = field.ParentClasses,
                                                                                                             UsingStatements = field.UsingStatements
                                                                                                         };

                                                             if (!map.TryGetValue(key,
                                                                                  out ImmutableArray<AlwaysInitializedPropertyFieldDefinition>.Builder builder))
                                                             {
                                                                 builder = ImmutableArray.CreateBuilder<AlwaysInitializedPropertyFieldDefinition>();

                                                                 map.Add(key,
                                                                         builder);
                                                             }

                                                             builder.Add(field);
                                                         }

                                                         token.ThrowIfCancellationRequested();

                                                         ImmutableArray<AlwaysInitializedPropertyClassDefinition>.Builder result = ImmutableArray.CreateBuilder<AlwaysInitializedPropertyClassDefinition>();

                                                         foreach (KeyValuePair<ClassWithParentAndNamespaceDefinition, ImmutableArray<AlwaysInitializedPropertyFieldDefinition>.Builder> entry in map)
                                                         {
                                                             AlwaysInitializedPropertyClassDefinition newEntry = new AlwaysInitializedPropertyClassDefinition();
                                                             newEntry.AdvClassDef = entry.Key;
                                                             newEntry.Fields = entry.Value.ToImmutable();

                                                             result.Add(newEntry);
                                                         }

                                                         return result;
                                                     });

            context.RegisterSourceOutput(groupedFields,
                                         static (spc, source) => Execute(source, spc));
        }

        private static void Execute(AlwaysInitializedPropertyClassDefinition groupedFields, SourceProductionContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var sb = new IndentedStringBuilder();
            var classBuilder = new AlwaysInitializedPropertyClassBuilder();
            classBuilder.Generate(sb, ref groupedFields);

            var filename = GenericClassBuilder.GenerateFilename(groupedFields.AdvClassDef.ClassDef.Name,
                                                                groupedFields.AdvClassDef.ClassDef,
                                                                groupedFields.AdvClassDef.Namespace!,
                                                                groupedFields.AdvClassDef.ParentClasses);

            context.AddSource(filename,
                              SourceText.From(sb.ToString(),
                                              Encoding.UTF8));

        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return syntaxNode.Parent is VariableDeclarationSyntax { Parent: FieldDeclarationSyntax }
                    && syntaxNode.IsKind(SyntaxKind.VariableDeclarator); // validating the parent first to help with code coverage
        }

        private static AlwaysInitializedPropertyFieldDefinition GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext ctx, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var fieldSyntax = (FieldDeclarationSyntax)ctx.TargetNode.Parent!.Parent!;

            var def = new AlwaysInitializedPropertyFieldDefinition();

            PopulateTarget(ctx,
                           fieldSyntax,
                           ref def);

            return def;
        }

        internal static bool PopulateTarget(GeneratorAttributeSyntaxContext context, 
                                            FieldDeclarationSyntax target, 
                                            ref AlwaysInitializedPropertyFieldDefinition def)
        {
            var cls = (target.Parent as ClassDeclarationSyntax)!;

            def.Name = GenerationHelpers.GetFieldIdentifierText(target);
            def.Type = target.Declaration.Type.ToString();
            def.LeadingTrivia = target.GetLeadingTrivia().ToString();

            if (target.Parent is not ClassDeclarationSyntax)
            {
                def.SetDiagnostic(new DiagnosticDefinition
                                  {
                                      Diagnostic = AlwaysInitializedPropertyGeneratorDescriptors.FieldMustBeInClass,
                                      Location = GenerationHelpers.GetFieldIdentifierLocation(target),
                                      Parameter = def.Name,
                                      Initialized = true
                                  });

                return false;
            }

            def.ClassDef = new ClassDefinition
                           {
                               Name = cls.Identifier.ValueText,
                               Modifier = String.Join(" ", cls.Modifiers.Select(x => x.ValueText))
                           };

            if (!GenerationHelpers.IsPartialType(cls))
            {
                def.SetDiagnostic(new DiagnosticDefinition
                                  {
                                      Diagnostic = AlwaysInitializedPropertyGeneratorDescriptors.MustBeInParentPartialClass,
                                      Location = GenerationHelpers.GetFieldIdentifierLocation(target),
                                      Parameter = def.Name,
                                      Initialized = true
                                  });

                return false;
            }

            var parentClasses = target.Ancestors().Where(x => x.IsKind(SyntaxKind.ClassDeclaration)).OfType<ClassDeclarationSyntax>().Skip(1).Reverse();

            foreach (var parentClass in parentClasses)
            {
                if (!GenerationHelpers.IsPartialType(parentClass))
                {
                    def.SetDiagnostic(new DiagnosticDefinition
                                      {
                                          Diagnostic = AlwaysInitializedPropertyGeneratorDescriptors.MustBeInParentPartialClass,
                                          Location = GenerationHelpers.GetFieldIdentifierLocation(target),
                                          Parameter = def.Name,
                                          Initialized = true
                                      });

                    return false;
                }

                def.ParentClasses ??= new List<ClassDefinition>();

                def.ParentClasses.Add(new ClassDefinition
                                      {
                                          Name = parentClass.Identifier.ValueText,
                                          Modifier = String.Join(" ",
                                                                 parentClass.Modifiers.Select(x => x.ValueText))
                                      });
            }

            if (!target.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            {
                def.SetDiagnostic(new DiagnosticDefinition
                                  {
                                      Diagnostic = AlwaysInitializedPropertyGeneratorDescriptors.FieldMustBePrivate,
                                      Location = GenerationHelpers.GetFieldIdentifierLocation(target),
                                      Parameter = def.Name,
                                      Initialized = true
                                  });

                return false;
            }

            if (target.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
            {
                def.SetDiagnostic(new DiagnosticDefinition
                                  {
                                      Diagnostic = AlwaysInitializedPropertyGeneratorDescriptors.FieldCannotBeStatic,
                                      Location = GenerationHelpers.GetFieldIdentifierLocation(target),
                                      Parameter = def.Name,
                                      Initialized = true
                                  });

                return false;
            }

            if (target.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword)))
            {
                def.SetDiagnostic(new DiagnosticDefinition
                                  {
                                      Diagnostic = AlwaysInitializedPropertyGeneratorDescriptors.FieldCannotBeReadOnly,
                                      Location = GenerationHelpers.GetFieldIdentifierLocation(target),
                                      Parameter = def.Name,
                                      Initialized = true
                                  });

                return false;
            }

            if (target.Declaration.Variables.Count != 1)
            {
                def.SetDiagnostic(new DiagnosticDefinition
                                  {
                                      Diagnostic = AlwaysInitializedPropertyGeneratorDescriptors.OnlyOneFieldCanBeDeclared,
                                      Location = GenerationHelpers.GetFieldIdentifierLocation(target),
                                      Parameter = def.Name,
                                      Initialized = true
                                  });

                return false;
            }

            var parentClassSyntax = (ClassDeclarationSyntax)target.Parent;

            def.Namespace = GenerationHelpers.GetNamespace(parentClassSyntax);

            if (String.IsNullOrWhiteSpace(def.Namespace))
            {
                def.SetDiagnostic(new DiagnosticDefinition
                                  {
                                      Diagnostic = AlwaysInitializedPropertyGeneratorDescriptors.FieldMustBeInNamespace,
                                      Location = GenerationHelpers.GetFieldIdentifierLocation(target),
                                      Parameter = def.Name,
                                      Initialized = true
                                  });

                return false;
            }

            var sm = context.SemanticModel;
            var typeSymbol = sm.GetTypeInfo(target.Declaration.Type);

            if (typeSymbol.Type is IErrorTypeSymbol)
            {
                def.SetDiagnostic(new DiagnosticDefinition
                                  {
                                      Diagnostic = AlwaysInitializedPropertyGeneratorDescriptors.FieldTypeMustExist,
                                      Location = GenerationHelpers.GetFieldIdentifierLocation(target),
                                      Parameter = def.Name,
                                      Initialized = true
                                  });

                return false;
            }

            var stringSymbol = sm.Compilation.GetTypeByMetadataName("System.String");

            if (typeSymbol.Type is { IsValueType: true }
                || typeSymbol.Type!.Equals(stringSymbol, SymbolEqualityComparer.Default))
            {
                def.SetDiagnostic(new DiagnosticDefinition
                                  {
                                      Diagnostic = AlwaysInitializedPropertyGeneratorDescriptors.FieldTypeMustBeAReferenceType,
                                      Location = GenerationHelpers.GetFieldIdentifierLocation(target),
                                      Parameter = def.Name,
                                      Initialized = true
                                  });

                return false;
            }

            var members = typeSymbol.Type!.GetMembers(".ctor");

            if (!members.OfType<IMethodSymbol>().Any(x => x.Parameters.IsDefaultOrEmpty))
            {
                def.SetDiagnostic(new DiagnosticDefinition
                                  {
                                      Diagnostic = AlwaysInitializedPropertyGeneratorDescriptors.FieldTypeMustHaveParameterlessConstructor,
                                      Location = GenerationHelpers.GetFieldIdentifierLocation(target),
                                      Parameter = def.Name,
                                      Initialized = true
                                  });

                return false;
            }

            var usingStatements = cls.Ancestors().Where(x => x.IsKind(SyntaxKind.CompilationUnit)).OfType<CompilationUnitSyntax>();
            var hasUsigns = false;
            ImmutableArray<string>.Builder usingBuilder = ImmutableArray.CreateBuilder<string>();
            
            foreach (var usingStatement in usingStatements)
            {
                foreach (var usingDirectiveSyntax in usingStatement.Usings)
                {
                    hasUsigns = true;

                    usingBuilder.Add(usingDirectiveSyntax.Name.ToString());
                }
            }

            if (hasUsigns)
            {
                def.UsingStatements = usingBuilder.ToImmutable();
            }

            return true;
        }
    }
}
