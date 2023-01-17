using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NoWoL.SourceGenerators
{

    internal class AlwaysInitializedPropertyClassBuilder
    {
        internal GenericClassBuilderResult Generate(IndentedStringBuilder sb, AlwaysInitializedPropertyClassToGenerate classToGenerate)
        {
            var fileNamePrefix = classToGenerate.ClassDeclarationSyntax.Identifier.ValueText;

            return GenericClassBuilder.GenerateClass(sb,
                                                     classToGenerate.NameSpace!,
                                                     classToGenerate.ClassDeclarationSyntax,
                                                     fileNamePrefix,
                                                     (isb) => AddBody(isb, classToGenerate),
                                                     addUsings: isb =>
                                                                {
                                                                    var cuSyntaxes = classToGenerate.ClassDeclarationSyntax.Ancestors().Where(x => x.IsKind(SyntaxKind.CompilationUnit));
                                                                    var hasUsings = false;

                                                                    foreach (var cuSyntax in cuSyntaxes!.Cast<CompilationUnitSyntax>())
                                                                    {
                                                                        foreach (var usingDirectiveSyntax in cuSyntax.Usings)
                                                                        {
                                                                            hasUsings = true;
                                                                            isb.Add(usingDirectiveSyntax.ToString(),
                                                                                    addNewLine: true);
                                                                        }
                                                                    }

                                                                    if (hasUsings)
                                                                    {
                                                                        isb.Add(String.Empty,
                                                                                addNewLine: true);
                                                                    }
                                                                });
        }

        private void AddBody(IndentedStringBuilder sb, AlwaysInitializedPropertyClassToGenerate classToGenerate)
        {
            sb.Add($@"    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    {CodeGenAttribute}
    {GenerationHelpers.BuildTypeDefinition(classToGenerate.ClassDeclarationSyntax)}
    {{
", removeLastNewLines: false, addNewLine: true);

            AddFields(sb,
                      classToGenerate);

            sb.Add(@"
    }", addNewLine: true);
        }

        private void AddFields(IndentedStringBuilder sb, AlwaysInitializedPropertyClassToGenerate classToGenerate)
        {
            sb.IncreaseIndent();
            sb.IncreaseIndent();

            for (var i = 0; i < classToGenerate.Fields.Length; i++)
            {
                var field = classToGenerate.Fields[i];

                var fieldName = field.Declaration.Variables.First().Identifier.ValueText;
                var fieldNameClean = fieldName.TrimStart('_');
                var propertyName = fieldNameClean.Substring(0, 1).ToUpperInvariant() + fieldNameClean.Substring(1);
                var fieldType = field.Declaration.Type.ToString();

                if (field.HasLeadingTrivia)
                {
                    var trivia = field.GetLeadingTrivia().ToString();
                    var lines = trivia.Split(new string[] { "\r\n", "\n" },
                                             StringSplitOptions.None);

                    for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
                    {
                        var line = lines[lineIndex];
                        sb.Add(line.TrimStart(), addNewLine: (lineIndex + 1 < lines.Length));
                    }
                }

                sb.Add($"public {fieldType} {propertyName}", addNewLine: true);
                sb.Add("{", addNewLine: true);
                sb.Add("    get", addNewLine: true);
                sb.Add("    {", addNewLine: true);
                sb.Add($"        if ({fieldName} == default)", addNewLine: true);
                sb.Add("        {", addNewLine: true);
                sb.Add($"            {fieldName} = new {fieldType}();", addNewLine: true);
                sb.Add("        }", addNewLine: true);
                sb.Add("", addNewLine: true);
                sb.Add($"        return {fieldName};", addNewLine: true);
                sb.Add("    }", addNewLine: true);
                
                sb.Add($"    set {{ {fieldName} = value; }}", addNewLine: true);

                if (i + 1 < classToGenerate.Fields.Length)
                {
                    sb.Add("}", addNewLine: true);
                    //sb.Add("", addNewLine: true);
                }
                else
                {
                    sb.Add("}");
                }
            }

            sb.DecreaseIndent();
            sb.DecreaseIndent();
        }

        private static readonly string CodeGenAttribute = GetCodeGenAttribute();

        private static string GetCodeGenAttribute()
        {
            return $@"[System.CodeDom.Compiler.GeneratedCodeAttribute(""{nameof(AlwaysInitializedPropertyGenerator)}"", ""{typeof(AlwaysInitializedPropertyGenerator).Assembly.GetName().Version}"")]";
        }

        private static readonly Regex FormatterRegex = new Regex(@"\{(?<Formatter><[^<>{}]+>)?\s*(?<DataType>[^ {}]+)\s+(?<ParamName>[^{}]+)\}", RegexOptions.Compiled);
    }
}