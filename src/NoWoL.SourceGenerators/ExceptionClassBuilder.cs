using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;

namespace NoWoL.SourceGenerators
{
    internal class ExceptionClassBuilderResult
    {
        public string FileName { get; set; }
    }

    internal class ExceptionClassBuilder
    {
        internal ExceptionClassBuilderResult GenerateException(IndentedStringBuilder sb, ClassToGenerate classToGenerate)
        {
            sb.AppendLine($@"namespace {classToGenerate.NameSpace}
{{");
            var parentClasses = classToGenerate.ClassDeclarationSyntax.Ancestors().Where(x => CSharpExtensions.IsKind((SyntaxNode?)x,
                                                                                                     SyntaxKind.ClassDeclaration)).OfType<ClassDeclarationSyntax>().Reverse();
            var filenameStringBuilder = new StringBuilder();
            filenameStringBuilder.Append(classToGenerate.NameSpace).Append(classToGenerate.ClassSymbol.Name);

            foreach (var parentClass in parentClasses)
            {
                var buildClassDefinition = BuildClassDefinition(parentClass);
                filenameStringBuilder.Append(buildClassDefinition);

                sb.IncreaseIndent();
                sb.AppendLine(buildClassDefinition);
                sb.AppendLine($@"{{");
            }

            AddExceptionBody(sb, classToGenerate);

            if (sb.Indent > 0)
            {
                sb.AppendLine("", skipIndent: true);
                while (sb.Indent > 1)
                {
                    sb.DecreaseIndent();
                    sb.AppendLine($@"}}");
                }
                sb.DecreaseIndent();
                sb.Append($@"}}");
            }

            return new ExceptionClassBuilderResult
                   {
                       FileName = GenerateFileName(filenameStringBuilder, classToGenerate)
                   };
        }

        private string GenerateFileName(StringBuilder filenameStringBuilder, ClassToGenerate classToGenerate)
        {
            var name = $"{classToGenerate.ClassSymbol.Name}_{GenerationHelpers.Md5(filenameStringBuilder.ToString())}.g.cs";

            return name;
        }

        private static string BuildClassDefinition(ClassDeclarationSyntax classDef)
        {
            var modifiers = GetClassAccessModifiers(classDef, addTrailingSpace: true);
            var staticDef = GetModifier(classDef, SyntaxKind.StaticKeyword, addTrailingSpace: true);
            var partialDef = GetModifier(classDef, SyntaxKind.PartialKeyword, addTrailingSpace: true);
            var abstractDef = GetModifier(classDef, SyntaxKind.AbstractKeyword, addTrailingSpace: true);

            return $"{modifiers}{staticDef}{abstractDef}{partialDef}class {classDef.Identifier.Value}";
        }

        private void AddExceptionBody(IndentedStringBuilder sb, ClassToGenerate classToGenerate)
        {
            var className = classToGenerate.ClassSymbol.Name;

            sb.AppendLines($@"    // This is generated code
    [System.Serializable]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    {BuildClassDefinition(classToGenerate.ClassDeclarationSyntax)} : System.Exception
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

            var exceptionHelper = GenerateExceptionHelper(classToGenerate);
            if (exceptionHelper != null)
            {
                sb.AppendLines(exceptionHelper, removeLastNewLines: true);
            }

            sb.AppendLines(@"
    }
}", removeLastNewLines: true);
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

        private string GenerateExceptionHelper(ClassToGenerate classToGenerate)
        {
            var className = classToGenerate.ClassSymbol.Name;

            var standardMessage = GetExceptionAttributeMessage(classToGenerate);

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

        private string? GetExceptionAttributeMessage(ClassToGenerate classToGenerate)
        {
            if (classToGenerate.ExceptionAttribute.ConstructorArguments.Length != 1)
            {
                return null;
            }

            return classToGenerate.ExceptionAttribute.ConstructorArguments[0].Value as string;
        }
    }
}