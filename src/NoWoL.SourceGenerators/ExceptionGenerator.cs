using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoWoL.SourceGenerators
{
    [Generator]
    public class ExceptionGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization(initializationContext =>
                                                  {
                                                      var src = EmbeddedResourceLoader.Get(EmbeddedResourceLoader.ExceptionGeneratorAttributeFileName);
                                                      initializationContext.AddSource("ExceptionGeneratorAttribute.g.cs",
                                                                                      src);
                                                  });

            context.RegisterForSyntaxNotifications(() => new ExceptionGeneratorSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;

            if (context.SyntaxReceiver is not ExceptionGeneratorSyntaxReceiver syntaxReceiver || syntaxReceiver.Targets.Count == 0)
            {
                return;
            }

            var exceptionGeneratorAttr = compilation.GetTypeByMetadataName("NoWoL.SourceGenerators.ExceptionGeneratorAttribute");

            var sb = new IndentedStringBuilder();

            foreach (var target in syntaxReceiver.Targets)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var semanticModel = compilation.GetSemanticModel(target.SyntaxTree);
                var targetType = ModelExtensions.GetDeclaredSymbol(semanticModel,
                                                                   target);
                var exceptionGeneratorAttribute = GetExceptionGeneratorAttribute(targetType,
                                                                                 exceptionGeneratorAttr);
                if (exceptionGeneratorAttribute == null)
                {
                    continue;
                }

                var ns = GetNamespace(target);

                if (!ValidateTarget(context, target, ns))
                {
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb.AppendLine("");
                }

                GenerateException(sb,
                                  targetType,
                                  target,
                                  exceptionGeneratorAttribute,
                                  ns);
            }

            if (sb.Length > 0)
            {
                context.AddSource("ExceptionGenerator.g.cs",
                                  sb.ToString());
            }
        }

        private static bool ValidateTarget(GeneratorExecutionContext context, ClassDeclarationSyntax target, string ns)
        {
            if (!IsPartialClass(target))
            {
                context.ReportDiagnostic(Diagnostic.Create("EG01",
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
                context.ReportDiagnostic(Diagnostic.Create("EG02",
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
            if (parentClasses.Any(x => !IsPartialClass(x)))
            {
                context.ReportDiagnostic(Diagnostic.Create("EG03",
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

        private static bool IsPartialClass(ClassDeclarationSyntax target)
        {
            return target.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
        }

        private static string GetClassAccessModifiers(ClassDeclarationSyntax target, bool addTrailingSpace = false)
        {
            var modifiersSyntax = target.Modifiers.Where(m => m.IsKind(SyntaxKind.PrivateKeyword)
                                                                || m.IsKind(SyntaxKind.PublicKeyword)
                                                                || m.IsKind(SyntaxKind.ProtectedKeyword)
                                                                || m.IsKind(SyntaxKind.InternalKeyword));

            var modifiers = String.Join(" ",
                                        modifiersSyntax.Select(x => x.ValueText));

            if (!addTrailingSpace || String.IsNullOrWhiteSpace(modifiers))
            {
                return modifiers;
            }

            return modifiers + " ";
        }

        private static string GetModifier(ClassDeclarationSyntax target, SyntaxKind kind, bool addTrailingSpace = false)
        {
            var modifier = target.Modifiers.FirstOrDefault(m => m.IsKind(kind)).ValueText;

            if (!addTrailingSpace || String.IsNullOrWhiteSpace(modifier))
            {
                return modifier;
            }

            return modifier + " ";
        }

        private static AttributeData GetExceptionGeneratorAttribute(ISymbol targetType, INamedTypeSymbol excepGeneratorAttr)
        {
            return targetType.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Equals(excepGeneratorAttr, SymbolEqualityComparer.Default) ?? false);
        }

        private void GenerateException(IndentedStringBuilder sb, ISymbol targetType, ClassDeclarationSyntax target, AttributeData exceptionGeneratorAttribute, string ns)
        {
            var className = targetType.Name;

            sb.AppendLine($@"namespace {ns}
{{");
            var parentClasses = target.Ancestors().Where(x => x.IsKind(SyntaxKind.ClassDeclaration)).OfType<ClassDeclarationSyntax>().Reverse();

            foreach (var parentClass in parentClasses)
            {
                sb.IncreaseIndent();
                sb.AppendLine(BuildClassDefinition(parentClass));
                sb.AppendLine($@"{{");
            }

            AddExceptionBody(sb, className, exceptionGeneratorAttribute, target);

            if (sb.Indent > 0)
            {
                sb.AppendLine("", skipIndent: true);
                while (sb.Indent > 0)
                {
                    sb.DecreaseIndent();
                    sb.AppendLine($@"}}");
                }
            }
        }

        private static string BuildClassDefinition(ClassDeclarationSyntax classDef)
        {
            var modifiers = GetClassAccessModifiers(classDef, addTrailingSpace: true);
            var staticDef = GetModifier(classDef, SyntaxKind.StaticKeyword, addTrailingSpace: true);
            var partialDef = GetModifier(classDef, SyntaxKind.PartialKeyword, addTrailingSpace: true);
            var abstractDef = GetModifier(classDef, SyntaxKind.AbstractKeyword, addTrailingSpace: true);

            return $"{modifiers}{staticDef}{abstractDef}{partialDef}class {classDef.Identifier.Value}";
        }

        private void AddExceptionBody(IndentedStringBuilder sb, string className, AttributeData exceptionGeneratorAttribute, ClassDeclarationSyntax target)
        {
            sb.AppendLines($@"    // This is generated code
    [System.Serializable]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    {BuildClassDefinition(target)} : System.Exception
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {{
        /// <summary>
        /// Creates an instance of the <see cref=""{className}""/> class.
        /// </summary>
        public {className}()
        {{}}

        /// <summary>
        /// Creates an instance of the <see cref=""{className}""/> class.
        /// </summary>
        /// <param name=""message"">Message of the exception</param>
        public {className}(string message)
            : base(message)
        {{}}

        /// <summary>
        /// Creates an instance of the <see cref=""{className}""/> class.
        /// </summary>
        /// <param name=""message"">Message of the exception</param>
        /// <param name=""innerException"">Optional inner exception</param>
        public {className}(string message, System.Exception innerException)
            : base(message, innerException)
        {{}}

        /// <summary>
        /// Initializes a new instance of the <see cref=""{className}""/> class.
        /// </summary>
        /// <param name=""info"">Serialization info</param>
        /// <param name=""context"">Serialization context</param>
        protected {className}(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {{
        }}", removeLastNewLines: true);

            var exceptionHelper = GenerateExceptionHelper(className, exceptionGeneratorAttribute);
            if (exceptionHelper != null)
            {
                sb.AppendLines(exceptionHelper, removeLastNewLines: true);
            }

            sb.AppendLines(@"
    }
}", removeLastNewLines: true);
        }

        private static string GetExceptionAttributeMessage(AttributeData exceptionGeneratorAttribute)
        {
            if (exceptionGeneratorAttribute.ConstructorArguments.Length != 1)
            {
                return null;
            }

            return exceptionGeneratorAttribute.ConstructorArguments[0].Value as string;
        }

        private string GenerateExceptionHelper(string className, AttributeData exceptionGeneratorAttribute)
        {
            var standardMessage = GetExceptionAttributeMessage(exceptionGeneratorAttribute);

            if (standardMessage == null)
            {
                return null;
            }

            var parameters = new List<(string type, string name)>();
            var template = Regex.Replace(standardMessage,
                                         @"\{(?<Formatter><[^<>{}]+>)?\s*(?<DataType>[^ {}]+)\s+(?<ParamName>[^{}]+)\}",
                                         new MatchEvaluator(ReplaceParameterMatch));
            var methodParameters = parameters.Count == 0 ?
                                       String.Empty :
                                       String.Join(", ", parameters.Select(x => $"{x.type} {x.name}")) + ", ";

            return $@"

        /// <summary>
        /// Helper method to create the exception
        /// </summary>
        /// <param name=""innerException"">Optional inner exception</param>
        /// <returns>An instance of the <see cref=""{className}""/> exception</returns>
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        public static {className} Create({methodParameters}System.Exception innerException = null)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {{
#pragma warning disable CA1062 // Validate arguments of public methods
            return new {className}($""{template}"", innerException);
#pragma warning restore CA1062 // Validate arguments of public methods
        }}";

            string ReplaceParameterMatch(Match match)
            {
                {
                    var dataType = match.Groups["DataType"].Value;
                    var name = match.Groups["ParamName"].Value;

                    parameters.Add((dataType, name));

                    if (match.Groups["Formatter"].Success)
                    {
                        var formatter = match.Groups["Formatter"].Value.Trim(new[] { '<', '>' });

                        return "{" + formatter + "(" + name + ")" + "}";
                    }

                    return "{" + name + "}";
                }
            }
        }

        private static string GetNamespace(ClassDeclarationSyntax target)
        {
            if (target.AncestorsAndSelf().FirstOrDefault(x => x.IsKind(SyntaxKind.NamespaceDeclaration)) is NamespaceDeclarationSyntax ns)
            {
                return ns.Name.ToString();
            }

            if (target.AncestorsAndSelf().FirstOrDefault(x => x.IsKind(SyntaxKind.FileScopedNamespaceDeclaration)) is FileScopedNamespaceDeclarationSyntax scopedNs)
            {
                return scopedNs.Name.ToString();
            }

            return null;
        }

        class ExceptionGeneratorSyntaxReceiver : ISyntaxReceiver
        {
            public HashSet<ClassDeclarationSyntax> Targets { get; } = new HashSet<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax cds
                    && !String.Equals(cds.Identifier.ValueText, "ExceptionGeneratorAttribute", StringComparison.Ordinal)
                    && cds.AttributeLists.Any())
                {
                    Targets.Add(cds);
                }
            }
        }
    }
}