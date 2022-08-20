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
    internal class GenericClassBuilder
    {
        internal static ExceptionClassBuilderResult GenerateClass(IndentedStringBuilder sb, 
                                                                  string nameSpace, 
                                                                  ClassDeclarationSyntax classDeclaration,
                                                                  string fileNamePrefix,
                                                                  Action<IndentedStringBuilder> addBody,
                                                                  Action<IndentedStringBuilder>? preAction = null)
        {
            preAction?.Invoke(sb);

            sb.Add($@"namespace {nameSpace}
{{", addNewLine: true);
            var parentClasses = classDeclaration.Ancestors().Where(x => CSharpExtensions.IsKind((SyntaxNode?)x,
                                                                                                SyntaxKind.ClassDeclaration)).OfType<ClassDeclarationSyntax>().Reverse();
            var filenameStringBuilder = new StringBuilder();
            filenameStringBuilder.Append(nameSpace).Append(classDeclaration.Identifier.ValueText);

            foreach (var parentClass in parentClasses)
            {
                var buildClassDefinition = GenerationHelpers.BuildClassDefinition(parentClass);
                filenameStringBuilder.Append(buildClassDefinition);

                sb.IncreaseIndent();
                sb.Add(buildClassDefinition, addNewLine: true);
                sb.Add($@"{{", addNewLine: true);
            }

            addBody(sb);

            if (sb.Indent > 0)
            {
                //sb.Add("", addNewLine: true);
                while (sb.Indent > 1)
                {
                    sb.Add($@"}}", addNewLine: true);
                    sb.DecreaseIndent();
                }

                sb.Add("}", addNewLine: true);
                sb.DecreaseIndent();
            }

            sb.Add("}");

            return new ExceptionClassBuilderResult
                   {
                       FileName = GenerateFileName(filenameStringBuilder, fileNamePrefix)
                   };
        }

        private static string GenerateFileName(StringBuilder filenameStringBuilder, string fileNamePrefix)
        {
            var name = $"{fileNamePrefix}_{GenerationHelpers.Md5(filenameStringBuilder.ToString())}.g.cs";

            return name;
        }
    }

    internal class ExceptionClassBuilder
    {
        internal ExceptionClassBuilderResult GenerateException(IndentedStringBuilder sb, ClassToGenerate classToGenerate)
        {
            var fileNamePrefix = classToGenerate.ClassDeclarationSyntax.Identifier.ValueText;

            return GenericClassBuilder.GenerateClass(sb,
                                                     classToGenerate.NameSpace!,
                                                     classToGenerate.ClassDeclarationSyntax,
                                                     fileNamePrefix,
                                                     (isb) => AddExceptionBody(isb, classToGenerate));
        }

        private void AddExceptionBody(IndentedStringBuilder sb, ClassToGenerate classToGenerate)
        {
            var className = classToGenerate.ClassSymbol.Name;

            sb.Add($@"    // This is generated code
    [System.Serializable]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    {CodeGenAttribute}
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    {GenerationHelpers.BuildClassDefinition(classToGenerate.ClassDeclarationSyntax)} : System.Exception
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
        }}", removeLastNewLines: true, addNewLine: true);

            AddExceptionHelpers(sb, classToGenerate);

            sb.Add(@"
    }", addNewLine: true);
        }

        private static readonly string CodeGenAttribute = GetCodeGenAttribute();

        private static string GetCodeGenAttribute()
        {
            return $@"[System.CodeDom.Compiler.GeneratedCodeAttribute(""{nameof(ExceptionClassGenerator)}"", ""{typeof(ExceptionClassGenerator).Assembly.GetName().Version}"")]";
        }

        private void AddExceptionHelpers(IndentedStringBuilder sb, ClassToGenerate classToGenerate)
        {
            var className = classToGenerate.ClassSymbol.Name;

            var standardMessages = GetExceptionAttributeMessages(classToGenerate);

            if (standardMessages.Count == 0)
            {
                return;
            }

            var parameters = new List<(string type, string name)>();
            foreach (var standardMessage in standardMessages)
            {
                parameters.Clear();

                var template = Regex.Replace(standardMessage,
                                             @"\{(?<Formatter><[^<>{}]+>)?\s*(?<DataType>[^ {}]+)\s+(?<ParamName>[^{}]+)\}",
                                             new MatchEvaluator(ReplaceParameterMatch));
                var methodParameters = parameters.Count == 0 ?
                                           String.Empty :
                                           String.Join(", ", parameters.Select(x => $"{x.type} {x.name}")) + ", ";

                sb.Add(Create(template, methodParameters),
                       removeLastNewLines: true,
                       addNewLine: true);
            }

            string Create(string template, string methodParameters)
            {
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
            }

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

        private static List<string> GetExceptionAttributeMessages(ClassToGenerate classToGenerate)
        {
            var messages = new List<string>();

            foreach (var exceptionAttribute in classToGenerate.ExceptionAttributes)
            {
                if (exceptionAttribute.ConstructorArguments.Length == 1)
                {
                    var msg = exceptionAttribute.ConstructorArguments[0].Value as string;

                    if (!String.IsNullOrWhiteSpace(msg))
                    {
                        messages.Add(msg!);
                    }
                }
            }

            return messages;
        }
    }
}