using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace NoWoL.SourceGenerators.Tests
{
    public abstract class BaseAnalyzerTests<T> where T: DiagnosticAnalyzer
    {
        private string GetDefaultSourceCode(string testMethodName)
        {
            const string folder = "ExceptionGeneratorAnalyzer";

            var initialCode = EmbeddedResourceLoader.Get(typeof(ExceptionGeneratorTests).Assembly,
                                                         $"NoWoL.SourceGenerators.Tests.Content.TestFiles.{folder}.{testMethodName}.Initial.cs");

            if (initialCode == null)
            {
                throw new ArgumentException($"Could not find an initial file for {testMethodName}",
                                            nameof(testMethodName));
            }

            return initialCode;
        }

        protected abstract (string attrFileName, string fileContent) GetAttributeFileContent();

        protected async Task Run(List<DiagnosticResult>? expectedDiagnosticResults = null,
                                 [CallerMemberName] string callerMemberName = "")
        {
            var test = new CSharpAnalyzerTest<ExceptionGeneratorAnalyzer, XUnitVerifier>();
            var attrFile = GetAttributeFileContent();
            test.TestState.Sources.Add(attrFile);
            test.TestCode = GetDefaultSourceCode(callerMemberName);

            if (expectedDiagnosticResults?.Count > 0)
            {
                test.ExpectedDiagnostics.AddRange(expectedDiagnosticResults);
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}