
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.CSharpGeneratorVerifier<NoWoL.SourceGenerators.ExceptionClassGenerator>;

namespace NoWoL.SourceGenerators.Tests
{

    // The generated files must appear in the correct order expected by the test runner. You may need to reorder the classes in your Initial file to match the generated output

    public class ExceptionGeneratorTests : BaseGeneratorTests<NoWoL.SourceGenerators.ExceptionClassGenerator>
    {
        public ExceptionGeneratorTests()
            : base("Exception")
        {
            
        }

        protected override (string attrFileName, string fileContent) GetAttributeFileContent()
        {
            var attrCode = EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoader).Assembly,
                                                      EmbeddedResourceLoader.ExceptionGeneratorAttributeFileName)!;
            return ("ExceptionGeneratorAttributeFqn.g.cs", attrCode);
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
                DiagnosticResult.CompilerError("NWL1003")
                                .WithMessage("The class 'TestClass' must be partial to use [ExceptionGenerator]")
                                .WithSpan(4, 18, 4, 27)
                                .WithArguments("TestClass")
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
            await WithWithEmbeddedFiles(additionalSourceFiles: new List<string> { "NoWoL.SourceGenerators.Tests.Content.TestFiles.Exception.ValueToStringTransformer.cs" }).ConfigureAwait(false);
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
                                                                       DiagnosticResult.CompilerError("NWL1002")
                                                                                       .WithMessage("The class 'TestClassScoped' must be contained in a namespace")
                                                                                       .WithSpan(2, 22, 2, 37)
                                                                                       .WithArguments("TestClassScoped")
                                                                   }).ConfigureAwait(false);
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
                                                                       DiagnosticResult.CompilerError("NWL1001")
                                                                                       .WithMessage("The parent classes of class 'TestClass' must also be partial")
                                                                                       .WithSpan(8, 34, 8, 43)
                                                                                       .WithArguments("TestClass")
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

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ExceptionCannotBeDuplicated()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerWarning("CS8785")
                                                                                       .WithMessage("Generator 'ExceptionClassGenerator' failed to generate source. It will not contribute to the output and compilation errors may occur as a result. Exception was of type 'ArgumentException' with message 'The hintName 'TestClass_ff77a22886df145d140e4b748d44b619.g.cs' of the added source file must be unique within a generator. (Parameter 'hintName')'")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MultipleCreationMessages()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }
    }
}