using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{
    public class ExceptionGeneratorAnalyzerTests : BaseAnalyzerTests<ExceptionGeneratorAnalyzer>
    {
        protected override (string attrFileName, string fileContent) GetAttributeFileContent()
        {
            var attrCode = EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoader).Assembly,
                                                      EmbeddedResourceLoader.ExceptionGeneratorAttributeFileName)!;
            return ("ExceptionGeneratorAttributeFqn.g.cs", attrCode);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task NoDiagnosticsForClassNotUsingAttribute()
        {
            await Run().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task NoDiagnosticsForPartialClass()
        {
            await Run().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task AttributeNotOnAClass()
        {
            await Run(expectedDiagnosticResults: new List<DiagnosticResult>
                                                 {
                                                     DiagnosticResult.CompilerError("CS0592")
                                                                     .WithMessage("Attribute 'NoWoL.SourceGenerators.ExceptionGenerator' is not valid on this declaration type. It is only valid on 'class' declarations.")
                                                                     .WithSpan("/0/Test1.cs", 3, 6, 3, 47)
                                                                     .WithArguments("NoWoL.SourceGenerators.ExceptionGenerator", "class")
                                                 }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task DifferentAttributeWithSameName()
        {
            await Run().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task RaisesDiagnostic()
        {
            // only testing one diagnostic here because the validation code is shared between the analyzer and the generator

            await Run(expectedDiagnosticResults: new List<DiagnosticResult>
                                                 {
                                                     DiagnosticResult.CompilerError("NWL1003")
                                                                     .WithMessage("The class 'TestClass' must be partial to use [ExceptionGenerator]")
                                                                     .WithSpan("/0/Test1.cs", 4, 18, 4, 27)
                                                                     .WithArguments("TestClass")
                                                 }).ConfigureAwait(false);
        }
    }
}
