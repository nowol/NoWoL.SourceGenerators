﻿using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
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
        public void IsPartialTypeReturnsFalseIfSyntaxIsNull()
        {
            Assert.False(SourceGenerators.GenerationHelpers.IsPartialType(null));
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
        public void ConvertErrorCodeShouldThrowForInvalidValue_AlwaysInitializedPropertyGenerator()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SourceGenerators.GenerationHelpers.ConvertErrorCode((AlwaysInitializedPropertyGeneratorErrorCode)66666));
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
        public void IsPartialTypeReturnsFalseWithNull()
        {
            var result = SourceGenerators.GenerationHelpers.IsPartialType(null);
            Assert.False(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsPartialTypeReturnsFalseWithNonPartialClass()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            var result = SourceGenerators.GenerationHelpers.IsPartialType(clsNode);
            Assert.False(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsPartialTypeReturnsTrueWithPartialClass()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            var result = SourceGenerators.GenerationHelpers.IsPartialType(clsNode);
            Assert.True(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsPartialTypeReturnsFalseWithNonPartialInterface()
        {
            var interfaceNode = SyntaxFactory.InterfaceDeclaration("IInterface").AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            var result = SourceGenerators.GenerationHelpers.IsPartialType(interfaceNode);
            Assert.False(result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsPartialTypeReturnsTrueWithPartialInterface()
        {
            var interfaceNode = SyntaxFactory.InterfaceDeclaration("IInterface").AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            var result = SourceGenerators.GenerationHelpers.IsPartialType(interfaceNode);
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
        public void BuildTypeDefinitionReturnsClassDefinitionWithBuiltInName()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            var result = SourceGenerators.GenerationHelpers.BuildTypeDefinition(clsNode);
            Assert.Equal("partial class ClassName", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void BuildTypeDefinitionReturnsClassDefinitionWithCustomName()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            var result = SourceGenerators.GenerationHelpers.BuildTypeDefinition(clsNode, "AnotherName");
            Assert.Equal("partial class AnotherName", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void BuildTypeDefinitionReturnsInterfaceDefinitionWithBuiltInName()
        {
            var interfaceNode = SyntaxFactory.InterfaceDeclaration("IInterfaceName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            var result = SourceGenerators.GenerationHelpers.BuildTypeDefinition(interfaceNode);
            Assert.Equal("partial interface IInterfaceName", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void BuildTypeDefinitionReturnsInterfaceDefinitionWithCustomName()
        {
            var interfaceNode = SyntaxFactory.InterfaceDeclaration("IInterfaceName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            var result = SourceGenerators.GenerationHelpers.BuildTypeDefinition(interfaceNode, "AnotherName");
            Assert.Equal("partial interface AnotherName", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void BuildClassDefinitionWithStringReturnsClassWithoutModifierEmpty()
        {
            var result = SourceGenerators.GenerationHelpers.BuildClassDefinition("classy", "");
            Assert.Equal("class classy", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void BuildClassDefinitionWithStringReturnsClassWithoutModifierNull()
        {
            var result = SourceGenerators.GenerationHelpers.BuildClassDefinition("classy", null);
            Assert.Equal("class classy", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void BuildClassDefinitionWithStringReturnsClassWithModifier()
        {
            var result = SourceGenerators.GenerationHelpers.BuildClassDefinition("classy", "public");
            Assert.Equal("public class classy", result);
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
            var result = SourceGenerators.GenerationHelpers.GetTypeAccessModifiers(clsNode, true);
            Assert.Equal("public ", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetClassAccessModifiersWithoutModifiersReturnEmpty()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName");
            var result = SourceGenerators.GenerationHelpers.GetTypeAccessModifiers(clsNode, true);
            Assert.Equal("", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetClassAccessModifiersWithoutTrailingSpace()
        {
            var clsNode = SyntaxFactory.ClassDeclaration("ClassName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            var result = SourceGenerators.GenerationHelpers.GetTypeAccessModifiers(clsNode, false);
            Assert.Equal("public", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetInterfaceAccessModifiersWithTrailingSpace()
        {
            var interfaceNode = SyntaxFactory.InterfaceDeclaration("IInterfaceName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            var result = SourceGenerators.GenerationHelpers.GetTypeAccessModifiers(interfaceNode, true);
            Assert.Equal("public ", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetInterfaceAccessModifiersWithoutModifiersReturnEmpty()
        {
            var interfaceNode = SyntaxFactory.InterfaceDeclaration("IInterfaceName");
            var result = SourceGenerators.GenerationHelpers.GetTypeAccessModifiers(interfaceNode, true);
            Assert.Equal("", result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetInterfaceAccessModifiersWithoutTrailingSpace()
        {
            var interfaceNode = SyntaxFactory.InterfaceDeclaration("IInterfaceName").AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            var result = SourceGenerators.GenerationHelpers.GetTypeAccessModifiers(interfaceNode, false);
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

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(AlwaysInitializedPropertyGeneratorErrorCode.MustBeInParentPartialClass, "NWL1101")]
        [InlineData(AlwaysInitializedPropertyGeneratorErrorCode.FieldMustBePrivate, "NWL1102")]
        [InlineData(AlwaysInitializedPropertyGeneratorErrorCode.FieldCannotBeStatic, "NWL1103")]
        [InlineData(AlwaysInitializedPropertyGeneratorErrorCode.ClassMustBeInNamespace, "NWL1104")]
        [InlineData(AlwaysInitializedPropertyGeneratorErrorCode.FieldCannotBeReadOnly, "NWL1105")]
        [InlineData(AlwaysInitializedPropertyGeneratorErrorCode.FieldMustBeInClass, "NWL1106")]
        [InlineData(AlwaysInitializedPropertyGeneratorErrorCode.FieldTypeMustBeAReferenceType, "NWL1107")]
        [InlineData(AlwaysInitializedPropertyGeneratorErrorCode.FieldTypeMustHaveParameterlessConstructor, "NWL1108")]
        [InlineData(AlwaysInitializedPropertyGeneratorErrorCode.FieldTypeMustExist, "NWL1109")]
        [InlineData(AlwaysInitializedPropertyGeneratorErrorCode.OnlyOneFieldCanBeDeclared, "NWL1110")]
        public void AlwaysInitializedPropertyGeneratorErrorCodeConvertErrorCodeMapping(object code, string expected)
        {
            var result = SourceGenerators.GenerationHelpers.ConvertErrorCode((AlwaysInitializedPropertyGeneratorErrorCode)code);
            Assert.Equal(expected, result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ConvertAlwaysInitializedPropertyGeneratorErrorCodeThrowsForOutOfRangeValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SourceGenerators.GenerationHelpers.ConvertErrorCode((AlwaysInitializedPropertyGeneratorErrorCode)666));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetFieldIdentifierTextReturnsTextOfFirstVariable()
        {
            var field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("System.String"),
                                                                                         SyntaxFactory.SeparatedList(new [] { SyntaxFactory.VariableDeclarator("Texto") })));
            var value = SourceGenerators.GenerationHelpers.GetFieldIdentifierText(field);

            Assert.Equal("Texto",
                         value);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetFieldIdentifierTextReturnsDefaultValue()
        {
            var field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("System.String"),
                                                                                         SyntaxFactory.SeparatedList(Array.Empty<VariableDeclaratorSyntax>())));
            var value = SourceGenerators.GenerationHelpers.GetFieldIdentifierText(field);

            Assert.Equal("",
                         value);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetFieldIdentifierLocationReturnsLocationOfFirstVariable()
        {
            var field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("System.String"),
                                                                                         SyntaxFactory.SeparatedList(new [] { SyntaxFactory.VariableDeclarator("Texto") })));

            var fieldLocation = SourceGenerators.GenerationHelpers.GetFieldIdentifierLocation(field);

            Assert.Equal(TextSpan.FromBounds(13, 18),
                         fieldLocation!.SourceSpan);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetFieldIdentifierLocationReturnsDefaultValue()
        {
            var field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("System.String"),
                                                                                         SyntaxFactory.SeparatedList(Array.Empty<VariableDeclaratorSyntax>())));
            var value = SourceGenerators.GenerationHelpers.GetFieldIdentifierLocation(field);

            Assert.Null(value);
        }
    }
}