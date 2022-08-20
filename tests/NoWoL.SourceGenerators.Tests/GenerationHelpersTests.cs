using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Testing;
using Moq;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{
    public class GenerationHelpers
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void ConvertErrorCodeShouldThrowForInvalidValue_AsyncToSync()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SourceGenerators.GenerationHelpers.ConvertErrorCode((AsyncToSyncErrorCode)66666));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsPartialClassReturnsFalseIfSyntaxIsNull()
        {
            Assert.False(SourceGenerators.GenerationHelpers.IsPartialClass(null));
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(ExceptionGeneratorErrorCode.MustBeInParentPartialClass, "NWL1001")]
        [InlineData(ExceptionGeneratorErrorCode.MethodClassMustBeInNamespace, "NWL1002")]
        [InlineData(ExceptionGeneratorErrorCode.MethodMustBePartial, "NWL1003")]
        public void ExceptionGeneratorErrorCodeConvertErrorCodeMapping(object code, string expected)
        {
            var result = SourceGenerators.GenerationHelpers.ConvertErrorCode((ExceptionGeneratorErrorCode)code);
            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ConvertErrorCodeShouldThrowForInvalidValue_ExceptionGenerator()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SourceGenerators.GenerationHelpers.ConvertErrorCode((ExceptionGeneratorErrorCode)66666));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsOperationCanceledExceptionReturnsTrueForCanceledException()
        {
            var result = SourceGenerators.GenerationHelpers.IsOperationCanceledException(new OperationCanceledException());
            Assert.True(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsOperationCanceledExceptionReturnsFalseIfExceptionIsNotCanceledException()
        {
            var result = SourceGenerators.GenerationHelpers.IsOperationCanceledException(new InvalidCastException());
            Assert.False(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsClassReturnFalseWithNull()
        {
            var result = SourceGenerators.GenerationHelpers.IsClass(null);
            Assert.False(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsClassReturnTrueWithClass()
        {
            var mock = new Mock<INamedTypeSymbol>();
            mock.SetupGet(x => x.TypeKind).Returns(TypeKind.Class);
            var result = SourceGenerators.GenerationHelpers.IsClass(mock.Object);
            Assert.True(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsClassReturnFalseWithNotAClass()
        {
            var mock = new Mock<INamedTypeSymbol>();
            mock.SetupGet(x => x.TypeKind).Returns(TypeKind.Enum);
            var result = SourceGenerators.GenerationHelpers.IsClass(mock.Object);
            Assert.False(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetModifierShouldAddATrailingSpace()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            var result = SourceGenerators.GenerationHelpers.GetModifier(clsNode, SyntaxKind.PublicKeyword, true);
            Assert.Equal("public ", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetModifierShouldNotAddATrailingSpace()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            var result = SourceGenerators.GenerationHelpers.GetModifier(clsNode, SyntaxKind.PublicKeyword, false);
            Assert.Equal("public", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsPartialClassReturnsFalseWithNull()
        {
            var result = SourceGenerators.GenerationHelpers.IsPartialClass(null);
            Assert.False(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsPartialClassReturnsFalseWithNonPartialClass()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            var result = SourceGenerators.GenerationHelpers.IsPartialClass(clsNode);
            Assert.False(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsPartialClassReturnsTrueWithPartialClass()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            var result = SourceGenerators.GenerationHelpers.IsPartialClass(clsNode);
            Assert.True(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void Md5ReturnsHash()
        {
            var result = SourceGenerators.GenerationHelpers.Md5("hello");
            Assert.Equal("5d41402abc4b2a76b9719d911017c592", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void BuildClassDefinitionReturnsClassDefinitionWithBuiltInName()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            var result = SourceGenerators.GenerationHelpers.BuildClassDefinition(clsNode);
            Assert.Equal("partial class ClassName", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void BuildClassDefinitionReturnsClassDefinitionWithCustomName()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            var result = SourceGenerators.GenerationHelpers.BuildClassDefinition(clsNode, "AnotherName");
            Assert.Equal("partial class AnotherName", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void RemoveLastWordReturnsInitialStringWhenWordIsNotFound()
        {
            var result = SourceGenerators.GenerationHelpers.RemoveLastWord("hello there", "sometext");
            Assert.Equal("hello there", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void RemoveLastWordReturnsStringWithoutLastWord()
        {
            var result = SourceGenerators.GenerationHelpers.RemoveLastWord("hello there", "there");
            Assert.Equal("hello ", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetClassAccessModifiersWithTrailingSpace()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            var result = SourceGenerators.GenerationHelpers.GetClassAccessModifiers(clsNode, true);
            Assert.Equal("public ", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetClassAccessModifiersWithoutModifiersReturnEmpty()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName");
            var result = SourceGenerators.GenerationHelpers.GetClassAccessModifiers(clsNode, true);
            Assert.Equal("", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetClassAccessModifiersWithoutTrailingSpace()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            var result = SourceGenerators.GenerationHelpers.GetClassAccessModifiers(clsNode, false);
            Assert.Equal("public", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IdentifierEndsWithAsyncReturnTrue()
        {
            var syntax = SyntaxFactory.IdentifierName("MethodAsync");
            var result = SourceGenerators.GenerationHelpers.IdentifierEndsWithAsync(syntax);
            Assert.True(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IdentifierEndsWithAsyncReturnFalse()
        {
            var syntax = SyntaxFactory.IdentifierName("Method");
            var result = SourceGenerators.GenerationHelpers.IdentifierEndsWithAsync(syntax);
            Assert.False(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EndsWithAsyncReturnTrue()
        {
            var result = SourceGenerators.GenerationHelpers.EndsWithAsync("MethodAsync");
            Assert.True(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EndsWithAsyncReturnFalse()
        {
            var result = SourceGenerators.GenerationHelpers.EndsWithAsync("Method");
            Assert.False(result);
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(AsyncToSyncErrorCode.AwaitedMethodMustEndWithAsync, "NWL0001")]
        [InlineData(AsyncToSyncErrorCode.ReturnedMethodMustEndWithAsync, "NWL0002")]
        [InlineData(AsyncToSyncErrorCode.AttributeMustBeAppliedToAClassEndingWithAsync, "NWL0003")]
        [InlineData(AsyncToSyncErrorCode.AttributeMustBeAppliedToPartialClass, "NWL0004")]
        [InlineData(AsyncToSyncErrorCode.AttributeMustBeAppliedInPartialClassHierarchy, "NWL0005")]
        [InlineData(AsyncToSyncErrorCode.MethodMustBeInNameSpace, "NWL0006")]
        [InlineData(AsyncToSyncErrorCode.MethodMustReturnTask, "NWL0007")]
        public void AsyncToSyncErrorCodeConvertErrorCodeMapping(object code, string expected)
        {
            var result = SourceGenerators.GenerationHelpers.ConvertErrorCode((AsyncToSyncErrorCode)code);
            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ConvertErrorCodeThrowsForOutOfRangeValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SourceGenerators.GenerationHelpers.ConvertErrorCode((AsyncToSyncErrorCode)666));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetNamespaceReturnEmptyWithoutNamespace()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName");
            var result = SourceGenerators.GenerationHelpers.GetNamespace(clsNode);
            Assert.Equal("", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetNamespaceReturnOldStyleNamespace()
        {
            var ns1 = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("ns1"));
            var ns2 = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("ns2"));
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName");

            ns1 = ns1.AddMembers(clsNode);
            ns2 = ns2.AddMembers(ns1);
            clsNode = (ClassDeclarationSyntax)ns2.DescendantNodes().First(x => x.IsKind(SyntaxKind.ClassDeclaration));

            var result = SourceGenerators.GenerationHelpers.GetNamespace(clsNode);
            Assert.Equal("ns2.ns1", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetNamespaceReturnFileScopedNamespaceDeclarationSyntax()
        {
            var ns1 = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName("ns1"));
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName");

            ns1 = ns1.AddMembers(clsNode);
            clsNode = (ClassDeclarationSyntax)ns1.DescendantNodes().First(x => x.IsKind(SyntaxKind.ClassDeclaration));

            var result = SourceGenerators.GenerationHelpers.GetNamespace(clsNode);
            Assert.Equal("ns1", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetNamespaceReturnNestedClasses()
        {
            var ns1 = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName("ns1"));
            var clsNode1 = SyntaxFactory.ClassDeclaration("ClassName1");
            var clsNode2 = SyntaxFactory.ClassDeclaration("ClassName2");

            clsNode1 = clsNode1.AddMembers(clsNode2);

            ns1 = ns1.AddMembers(clsNode1);
            clsNode1 = (ClassDeclarationSyntax)ns1.DescendantNodes().Last(x => x.IsKind(SyntaxKind.ClassDeclaration));

            Assert.Equal("ClassName2",
                         clsNode1.Identifier.ValueText);

            var result = SourceGenerators.GenerationHelpers.GetNamespace(clsNode1);
            Assert.Equal("ns1", result);
        }
    }
}