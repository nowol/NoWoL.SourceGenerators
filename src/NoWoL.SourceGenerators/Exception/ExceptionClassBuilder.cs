using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NoWoL.SourceGenerators
{

    internal class ExceptionClassBuilder
    {
        internal GenericClassBuilderResult GenerateException(IndentedStringBuilder sb, ExceptionClassToGenerate classToGenerate)
        {
            var fileNamePrefix = classToGenerate.ClassDeclarationSyntax.Identifier.ValueText;

            return GenericClassBuilder.GenerateClass(sb,
                                                     classToGenerate.NameSpace!,
                                                     classToGenerate.ClassDeclarationSyntax,
                                                     fileNamePrefix,
                                                     (isb) => AddExceptionBody(isb, classToGenerate));
        }

        private void AddExceptionBody(IndentedStringBuilder sb, ExceptionClassToGenerate classToGenerate)
        {
            var className = classToGenerate.ClassDeclarationSyntax.Identifier.ToString();

            sb.Add($@"    [System.Serializable]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    {CodeGenAttribute}
    {GenerationHelpers.BuildTypeDefinition(classToGenerate.ClassDeclarationSyntax)} : System.Exception
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

        private static readonly Regex FormatterRegex = new Regex(@"\{(?<Formatter><[^<>{}]+>)?\s*(?<DataType>[^ {}]+)\s+(?<ParamName>[^{}]+)\}", RegexOptions.Compiled);

        private void AddExceptionHelpers(IndentedStringBuilder sb, ExceptionClassToGenerate classToGenerate)
        {
            var className = classToGenerate.ClassDeclarationSyntax.Identifier.ToString();

            var standardMessages = GetExceptionAttributeMessages(classToGenerate);

            if (standardMessages.Count == 0)
            {
                return;
            }

            var parameters = new List<(string type, string name)>();
            foreach (var standardMessage in standardMessages)
            {
                parameters.Clear();

                var template = FormatterRegex.Replace(standardMessage,
                                                      ReplaceParameterMatch);
                var methodParameters = parameters.Count == 0 ?
                                           String.Empty :
                                           String.Join(", ", parameters.Select(x => $"{x.type} {x.name}"));

                sb.Add(Create(template, methodParameters),
                       removeLastNewLines: true,
                       addNewLine: true);

                sb.Add(CreateMessage(template, methodParameters),
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
        public static {className} Create({methodParameters}{(String.IsNullOrWhiteSpace(methodParameters) ? "" : ", ")}System.Exception innerException = null)
        {{
            return new {className}($""{template}"", innerException);
        }}";
            }

            string CreateMessage(string template, string methodParameters)
            {
                return $@"

        /// <summary>
        /// Helper method to create the exception's message
        /// </summary>
        /// <returns>An string with the message of the <see cref=""{className}""/> exception</returns>
        public static string CreateMessage({methodParameters})
        {{
            return $""{template}"";
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

        private static List<string> GetExceptionAttributeMessages(ExceptionClassToGenerate classToGenerate)
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