using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using Xunit;
using VerifyCS = CSharpGeneratorVerifier<NoWoL.SourceGenerators.ExceptionGenerator>;

namespace NoWoL.SourceGenerators.Tests
{

    // The generated files must appear in the correct order expected by the test runner. You may need to reorder the classes in your Initial file to match the generated output

    public class ExceptionGeneratorTests
    {
        private async Task WithWithEmbeddedFiles(List<DiagnosticResult>? expectedDiagnosticResults = null,
                                                 List<string>? additionalSourceFiles = null,
                                                 [CallerMemberName] string callerMemberName = "")
        {
            if (string.IsNullOrWhiteSpace(callerMemberName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.",
                                            nameof(callerMemberName));
            }

            var initialCode = EmbeddedResourceLoader.Get(typeof(ExceptionGeneratorTests).Assembly, $"NoWoL.SourceGenerators.Tests.Content.TestFiles.{callerMemberName}.Initial.cs");

            if (initialCode == null)
            {
                throw new ArgumentException($"Could not find an initial file for {callerMemberName}",
                                            nameof(callerMemberName));
            }

            var generatedCodes = EmbeddedResourceLoader.GetFilesFromPartialName(typeof(ExceptionGeneratorTests).Assembly, "NoWoL.SourceGenerators.Tests.Content.TestFiles", $"{callerMemberName}.Generated.");
            var attrCode = EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoader).Assembly, 
                                                      EmbeddedResourceLoader.ExceptionGeneratorAttributeFileName)!;

            if (additionalSourceFiles != null)
            {
                foreach (string additionalFile in additionalSourceFiles)
                {
                    var content = EmbeddedResourceLoader.Get(typeof(ExceptionGeneratorTests).Assembly,
                                                             additionalFile);
                    initialCode += "\r\n" + content;
                }
            }

            var test = new VerifyCS.Test
            {
                TestState =
                           {
                               Sources = { initialCode },
                               GeneratedSources =
                               {
                                   (typeof(ExceptionGenerator), "ExceptionGeneratorAttributeFqn.g.cs", SourceText.From(attrCode, Encoding.UTF8, SourceHashAlgorithm.Sha1))
                               }
                           }
            };

            if (generatedCodes.Count > 0)
            {
                foreach (var generatedCode in generatedCodes.OrderBy(x => x.FileName))
                {
                    test.TestState.GeneratedSources.Add((typeof(ExceptionGenerator), $"{generatedCode.FileName}.g.cs", SourceText.From(generatedCode.Content!,
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

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ClassWithoutAttribute()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ExceptionClassWithAttributeWithoutMessageShouldSucceed()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task TwoExceptionClassesWithAttributeWithoutMessageShouldSucceed()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ExceptionClassWithAttributeWithoutPartialShouldFail()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
            {
                DiagnosticResult.CompilerError("EG01").WithMessage("The [ExceptionGenerator] must be applied to a partial class.").WithSpan(3, 5, 4, 31)
            }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ExceptionClassWithAttributeWithMessageShouldSucceed()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ExceptionClassWithAttributeWithParametrizedMessageShouldSucceed()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ExceptionClassWithAttributeWithTransformedParametrizedMessageShouldSucceed()
        {
            await WithWithEmbeddedFiles(additionalSourceFiles: new List<string> { "NoWoL.SourceGenerators.Tests.Content.TestFiles.ValueToStringTransformer.cs" }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ExceptionClassWithScopedFileNamespaceShouldSucceed()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ExceptionClassWithoutNamespaceShouldFail()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("EG02").WithMessage("The [ExceptionGenerator] must be applied to a partial class contained in a namespace.").WithSpan(1, 1, 2, 41)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task GeneratorShouldIgnoreAttributeWithSameAttributeName()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task AliasedAttributeShouldSucceed()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task NestedClassesShouldSucceed()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task NestedClassesShouldMustBePartial()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("EG03").WithMessage("The [ExceptionGenerator] must be applied to a partial class nested in another partial class.")
                                                                                       .WithSpan(7, 13, 10, 14)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ClassModifiersShouldBePreserved()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task TwoExceptionsInDifferentScopeWithSameNameShouldBeGenerated()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task TestedNamespaceShouldSucceed()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }
    }
}