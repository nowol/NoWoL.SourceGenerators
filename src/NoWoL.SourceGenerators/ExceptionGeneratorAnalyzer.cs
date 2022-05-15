﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NoWoL.SourceGenerators
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExceptionGeneratorAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(ExceptionGeneratorDescriptors.MethodMustBePartial,
                                                                                                                   ExceptionGeneratorDescriptors.MethodClassMustBeInNamespace,
                                                                                                                   ExceptionGeneratorDescriptors.MustBeInParentPartialClass);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            if (context.Symbol is not INamedTypeSymbol { TypeKind: TypeKind.Class } type)
            {
                return;
            }

            if (!IsValid(context.Symbol))
            {
                return;
            }

            foreach (var classDeclarationSyntax in FilterClassSyntax(type.DeclaringSyntaxReferences))
            {
                var ns = GenerationHelpers.GetNamespace(classDeclarationSyntax);
                if (!ExceptionGenerator.TryValidateTarget(classDeclarationSyntax, ns, out var diagnosticToReport))
                {
                    var error = Diagnostic.Create(diagnosticToReport!,
                                                  classDeclarationSyntax.Identifier.GetLocation(),
                                                  type.Name);
                    context.ReportDiagnostic(error);
                }
            }
        }

        private static IEnumerable<ClassDeclarationSyntax> FilterClassSyntax(ImmutableArray<SyntaxReference> typeDeclaringSyntaxReferences)
        {
            return typeDeclaringSyntaxReferences.Select(d => d.GetSyntax() as ClassDeclarationSyntax).Where(x => x is not null)!;
        }

        private static bool IsValid(ISymbol type)
        {
            return type.GetAttributes()
                       .Any(a => a.AttributeClass?.Name == "ExceptionGeneratorAttribute"
                                 && a.AttributeClass.ContainingNamespace is
                                 {
                                     Name: "SourceGenerators",
                                     IsGlobalNamespace: false
                                 }
                                 && a.AttributeClass.ContainingNamespace.ContainingNamespace is
                                 {
                                     Name: "NoWoL",
                                     IsGlobalNamespace: false
                                 }
                                 && a.AttributeClass.ContainingNamespace.ContainingNamespace.ContainingNamespace is
                                 {
                                     IsGlobalNamespace: true
                                 });
        }
    }
}
