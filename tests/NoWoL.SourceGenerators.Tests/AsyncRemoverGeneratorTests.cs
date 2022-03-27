using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{

    // The generated files must appear in the correct order expected by the test runner. You may need to reorder the classes in your Initial file to match the generated output

    public class AsyncRemoverGeneratorTests : BaseGeneratorTests<NoWoL.SourceGenerators.ExperimentalAsyncRemoverGenerator>
    {
        public AsyncRemoverGeneratorTests()
            : base("AsyncRemover")
        {

        }

        protected override (string attrFileName, string fileContent) GetAttributeFileContent()
        {
            var attrCode = EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoader).Assembly,
                                                      EmbeddedResourceLoader.ExperimentalAsyncRemoverAttributeFileName)!;
            return ("ExperimentalAsyncRemoverAttributeFqn.g.cs", attrCode);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MethodMustBeInPartialClass()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0004").WithMessage("The [ExperimentalAsyncRemover] must be applied to a partial class.").WithSpan(7, 9, 11, 10)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MethodMustBeInPartialClassNested()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0005").WithMessage("The [ExperimentalAsyncRemover] must be applied to a partial class nested in another partial class.").WithSpan(9, 13, 13, 14)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MethodMustEndWithAsync()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0003").WithMessage("The [ExperimentalAsyncRemover] must be applied to a method ending with Async.").WithSpan(7, 9, 11, 10)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MustBeInNamespace()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0006").WithMessage("The [ExperimentalAsyncRemover] must be applied to a method in a partial class contained in a namespace.").WithSpan(5, 5, 9, 6)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MustReturnATask()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0007").WithMessage("The [ExperimentalAsyncRemover] must be applied to a method return a task instead of type 'void'.").WithSpan(8, 16, 8, 20)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task TaskToVoidWithOneStatement()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task DifferentAccessModifiers()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ReturningAMethodWithoutAwaitingItMustEndsWithAsync()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0002").WithMessage("The returned method 'TheMethod' must end with 'Async'.").WithSpan(10, 13, 10, 32)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ReturnsTaskDirectly()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ReturnsValueTaskDirectly()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ReturnsGenericTaskDirectly()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ReturnsGenericValueTaskDirectly()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ReturnsNonAwaitedValueGenericTask()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task RemoveReturnStatement()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ReturnsAwaitedValueGenericTask()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task AsyncWithConfigureAwaitInsideAPredicateShouldEndWithAsync()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0001").WithMessage("The awaited method 'SimulateWork' must end with 'Async'.").WithSpan(10, 46, 10, 92)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task AsyncWithoutConfigureAwaitInsideAPredicateShouldEndWithAsync()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0001").WithMessage("The awaited method 'SimulateWork' must end with 'Async'.").WithSpan(10, 46, 10, 70)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task PredicateWithAwait()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task PredicateWithReturnedTask()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task CopyXmlDoc()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FuncParameters()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task LocalAsyncMethod()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task AsyncStreamsMethodMustEndWithAsync()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0001").WithMessage("The awaited method 'SimulateWork' must end with 'Async'.").WithSpan(10, 37, 10, 51)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task AsyncStreams()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task WithConfigureAwait()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task WithConfigureAwaitWithCulture()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ConvertTaskDelayToThreadSleep()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task EveryCaseAtOnce()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task AttributeWithoutMethod()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MoreThanOneAttribute()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MoreThanOneAttributeWithXmlDoc()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task AnotherAttributeWithSameName()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }
    }
}