using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace NoWoL.SourceGenerators.Tests
{
    public abstract class BaseGeneratorTests<T>
        where T : IIncrementalGenerator, new()
    {
        private readonly string _folder;

        protected BaseGeneratorTests(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentException("Value cannot be null or whitespace.",
                                            nameof(folder));
            }

            _folder = folder;
        }

        protected async Task WithWithEmbeddedFiles(List<DiagnosticResult>? expectedDiagnosticResults = null,
                                                   List<string>? additionalSourceFiles = null,
                                                   bool addGeneratedAttributeDefinitionFile = true,
                                                   bool enableNullable = true,
                                                   [CallerMemberName] string callerMemberName = "")
        {
            if (string.IsNullOrWhiteSpace(callerMemberName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.",
                                            nameof(callerMemberName));
            }

            var initialCode = EmbeddedResourceLoader.Get(typeof(ExceptionGeneratorTests).Assembly, $"NoWoL.SourceGenerators.Tests.Content.TestFiles.{_folder}.{callerMemberName}.Initial.cs");

            if (initialCode == null)
            {
                throw new ArgumentException($"Could not find an initial file for {callerMemberName}",
                                            nameof(callerMemberName));
            }

            var generatedCodes = EmbeddedResourceLoader.GetFilesFromPartialName(typeof(ExceptionGeneratorTests).Assembly, $"NoWoL.SourceGenerators.Tests.Content.TestFiles.{_folder}", $"{callerMemberName}.Generated.");

            if (additionalSourceFiles != null)
            {
                foreach (string additionalFile in additionalSourceFiles)
                {
                    var content = EmbeddedResourceLoader.Get(typeof(ExceptionGeneratorTests).Assembly,
                                                             additionalFile);
                    initialCode += "\r\n" + content;
                }
            }
            
            var test = new Microsoft.CodeAnalysis.CSharp.Testing.CSharpGeneratorVerifier<T>.Test
                       {
                           TestState =
                           {
                               Sources = { initialCode }
                           },
                           NullableContextOptions = enableNullable ? NullableContextOptions.Enable : NullableContextOptions.Disable,
                       };

            if (addGeneratedAttributeDefinitionFile)
            {
                var (attrFileName, attrCode) = GetAttributeFileContent();
                test.TestState.GeneratedSources.Add((typeof(T), attrFileName, SourceText.From(attrCode,
                                                                                              Encoding.UTF8,
                                                                                              SourceHashAlgorithm.Sha1)));
            }

            if (generatedCodes.Count > 0)
            {
                foreach (var generatedCode in generatedCodes.OrderBy(x => x.FileName))
                {
                    test.TestState.GeneratedSources.Add((typeof(T), $"{generatedCode.FileName}.g.cs", SourceText.From(generatedCode.Content!.Replace(@""", ""1.0.0.0"")]",
                                                                                                                                                     $@""", ""{typeof(AsyncToSyncConverterGenerator).Assembly.GetName().Version}"")]"),
                                                                                                                      Encoding.UTF8,
                                                                                                                      SourceHashAlgorithm.Sha1)));
                }
            }

            if (expectedDiagnosticResults != null)
            {
                test.TestState.ExpectedDiagnostics.AddRange(expectedDiagnosticResults);
            }

            await test.RunAsync();
        }

        protected abstract (string attrFileName, string fileContent) GetAttributeFileContent();
    }
}
