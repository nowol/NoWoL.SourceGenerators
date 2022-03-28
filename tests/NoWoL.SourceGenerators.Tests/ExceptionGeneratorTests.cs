﻿using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{

    // The generated files must appear in the correct order expected by the test runner. You may need to reorder the classes in your Initial file to match the generated output

    public class ExceptionGeneratorTests : BaseGeneratorTests<NoWoL.SourceGenerators.ExceptionGenerator>
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