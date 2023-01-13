using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{

    // The generated files must appear in the correct order expected by the test runner. You may need to reorder the classes in your Initial file to match the generated output

    public class AsyncRemoverGeneratorTests : BaseGeneratorTests<NoWoL.SourceGenerators.AsyncToSyncConverterGenerator>
    {
        public AsyncRemoverGeneratorTests()
            : base("AsyncRemover")
        {

        }

        protected override (string attrFileName, string fileContent) GetAttributeFileContent()
        {
            var attrCode = EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoader).Assembly,
                                                      EmbeddedResourceLoader.AsyncToSyncConverterAttributeFileName)!;
            return ("AsyncToSyncConverterAttributeFqn.g.cs", attrCode);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MethodMustBeInPartialClass()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0004").WithMessage("The [AsyncToSyncConverter] must be applied to a partial class.").WithSpan(7, 9, 11, 10)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MethodMustBeInPartialClassNested()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0005").WithMessage("The [AsyncToSyncConverter] must be applied to a partial class nested in another partial class.").WithSpan(9, 13, 13, 14)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MethodMustEndWithAsync()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0003").WithMessage("The [AsyncToSyncConverter] must be applied to a method ending with Async.").WithSpan(7, 9, 11, 10)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MustBeInNamespace()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0006").WithMessage("The [AsyncToSyncConverter] must be applied to a method in a partial class contained in a namespace.").WithSpan(5, 5, 9, 6)
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MustReturnATask()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL0007").WithMessage("The [AsyncToSyncConverter] must be applied to a method which returns a task (Task or ValueTask) instead of type 'void'.").WithSpan(8, 16, 8, 20)
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
        public async Task PredicateWithBlockReturnedTask()
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
        public async Task AsyncWithoutParameters()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task AsyncWithParameters()
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

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task MethodCannotBeDuplicated()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerWarning("CS8785")
                                                                                       .WithMessage("Generator 'AsyncToSyncConverterGenerator' failed to generate source. It will not contribute to the output and compilation errors may occur as a result. Exception was of type 'ArgumentException' with message 'The hintName 'TestClass_MainMethod_ff77a22886df145d140e4b748d44b619.g.cs' of the added source file must be unique within a generator. (Parameter 'hintName')'")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task WorksWithInterfaceMethods()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task WorksWithInterfaceMethodsInNestedClasses()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task RemoveCancellationToken()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }
    }
}