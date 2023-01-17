
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.CSharpGeneratorVerifier<NoWoL.SourceGenerators.AlwaysInitializedPropertyGenerator>;

namespace NoWoL.SourceGenerators.Tests
{
    // The generated files must appear in the correct order expected by the test runner. You may need to reorder the classes in your Initial file to match the generated output

    public class AlwaysInitializedPropertyTests : BaseGeneratorTests<NoWoL.SourceGenerators.AlwaysInitializedPropertyGenerator>
    {
        public AlwaysInitializedPropertyTests()
            : base("AlwaysInitializedProperty")
        {

        }

        protected override (string attrFileName, string fileContent) GetAttributeFileContent()
        {
            var attrCode = EmbeddedResourceLoader.Get(typeof(EmbeddedResourceLoader).Assembly,
                                                      EmbeddedResourceLoader.AlwaysInitializedPropertyAttributeFileName)!;
            return ("AlwaysInitializedPropertyAttribute.g.cs", attrCode);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task EmptyClass()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FieldWithoutAttribute()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task CannotBeInAStruct()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1106")
                                                                                       .WithMessage("The field '_field1' must be part of a class to use [AlwaysInitializedPropertyGenerator]")
                                                                                       .WithSpan(8, 27, 8, 34)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FieldMustBeInPartialClass()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1101")
                                                                                       .WithMessage("The parent classes of field '_field1' must all be partial")
                                                                                       .WithSpan(8, 27, 8, 34)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task EveryClassesMustBePartial()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1101")
                                                                                       .WithMessage("The parent classes of field '_field1' must all be partial")
                                                                                       .WithSpan(10, 31, 10, 38)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FieldCannotBeStatic()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1103")
                                                                                       .WithMessage("The field '_field1' cannot be static to use [AlwaysInitializedPropertyGenerator]")
                                                                                       .WithSpan(8, 34, 8, 41)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FieldCannotBeReadOnly()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1105")
                                                                                       .WithMessage("The field '_field1' cannot be readonly to use [AlwaysInitializedPropertyGenerator]")
                                                                                       .WithSpan(8, 36, 8, 43)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FieldCannotBeInternal()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1102")
                                                                                       .WithMessage("The field '_field1' must have a private access modifier to use [AlwaysInitializedPropertyGenerator]")
                                                                                       .WithSpan(8, 28, 8, 35)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FieldCannotBePublic()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1102")
                                                                                       .WithMessage("The field '_field1' must have a private access modifier to use [AlwaysInitializedPropertyGenerator]")
                                                                                       .WithSpan(8, 26, 8, 33)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FieldCannotBeProtected()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1102")
                                                                                       .WithMessage("The field '_field1' must have a private access modifier to use [AlwaysInitializedPropertyGenerator]")
                                                                                       .WithSpan(8, 29, 8, 36)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FieldMustHaveAPrivateAccessModifier()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1102")
                                                                                       .WithMessage("The field '_field1' must have a private access modifier to use [AlwaysInitializedPropertyGenerator]")
                                                                                       .WithSpan(8, 19, 8, 26)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FieldTypeCannotBeAValueTypeBool()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1107")
                                                                                       .WithMessage("The type of field '_field1' must be a reference type")
                                                                                       .WithSpan(8, 22, 8, 29)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FieldTypeCannotBeAValueTypeString()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1107")
                                                                                       .WithMessage("The type of field '_field1' must be a reference type")
                                                                                       .WithSpan(8, 24, 8, 31)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FieldTypeMustHaveAParameterlessConstructor()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1108")
                                                                                       .WithMessage("The type of field '_field1' must have a parameterless constructor")
                                                                                       .WithSpan(8, 25, 8, 32)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task FieldTypeMustHaveAValidType()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1109")
                                                                                       .WithMessage("The type of field '_field1' must exist")
                                                                                       .WithSpan(8, 29, 8, 36)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ClassMustBeInNamespace()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1104")
                                                                                       .WithMessage("The field '_field1' must be in a class contained in a namespace")
                                                                                       .WithSpan(6, 23, 6, 30)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task CannotDeclareMoreThanOneFieldAtTheSameTime()
        {
            await WithWithEmbeddedFiles(expectedDiagnosticResults: new List<DiagnosticResult>
                                                                   {
                                                                       DiagnosticResult.CompilerError("NWL1110")
                                                                                       .WithMessage("Declaration for field '_field1' cannot declare more than one variable")
                                                                                       .WithSpan(8, 27, 8, 34)
                                                                                       .WithArguments("_field1"),
                                                                       DiagnosticResult.CompilerError("NWL1110")
                                                                                       .WithMessage("Declaration for field '_field1' cannot declare more than one variable")
                                                                                       .WithSpan(8, 27, 8, 34)
                                                                                       .WithArguments("_field1")
                                                                   }).ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task GenerateClassWithOneField()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task GenerateClassWithMultipleFields()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task GenerateClassWithOneFieldNewNamespaceSyntax()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task GenerateClassWithOneFieldNestedNamespace()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task GenerateClassWithOneInitializedField()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task GenerateClassWithOneFieldWithXmlDoc()
        {
            await WithWithEmbeddedFiles().ConfigureAwait(false);
        }
    }
}