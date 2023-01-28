using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{
    public class ExceptionClassDefinitionTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void BlanksAreEquals()
        {
            var def1 = new ExceptionClassDefinition();
            var def2 = new ExceptionClassDefinition();

            Assert.Equal(def1,
                         def2);

            Assert.Equal(def1.GetHashCode(),
                         def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsObjectsReturnsTrue()
        {
            var def1 = new ExceptionClassDefinition();
            var def2 = new ExceptionClassDefinition();

            Assert.True(def1.Equals((object?)def2));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsObjectsReturnsFalseForDiffType()
        {
            var def1 = new ExceptionClassDefinition();
            var def2 = new object();

            Assert.False(def1.Equals((object?)def2));
        }

        private ExceptionClassDefinition Create()
        {
            return new ExceptionClassDefinition
                   { 
                       Namespace = "namespace",
                       ClassDef = new ClassDefinition { Name = "classy" },
                       DiagnosticDef = new DiagnosticDefinition { Initialized = true },
                       Messages = new List<string> { "msg1", "msg2" },
                       ParentClasses = new List<ClassDefinition> { new ClassDefinition { Name = "parent" } }
                   };
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreEqualsIfAllPropertiesAreFilled()
        {
            var def1 = Create();
            var def2 = Create();

            Assert.Equal(def1,
                         def2);

            Assert.Equal(def1.GetHashCode(),
                         def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreNotEqualsIfNamespaceIsDifferent()
        {
            var def1 = Create();
            def1.Namespace = "another ns";
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreNotEqualsIfLeftNamespaceIsNull()
        {
            var def1 = Create();
            def1.Namespace = null;
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreNotEqualsIfRightNamespaceIsNull()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Namespace = null;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreNotEqualsIfParentClassesIsDifferent()
        {
            var def1 = Create();
            def1.ParentClasses = new List<ClassDefinition> { new ClassDefinition { Name = "parent"}, new ClassDefinition { Name = "another parent" }};
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreNotEqualsIfLeftParentClassesIsNull()
        {
            var def1 = Create();
            def1.ParentClasses = null;
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreNotEqualsIfRightParentClassesIsNull()
        {
            var def1 = Create();
            var def2 = Create();
            def2.ParentClasses = null;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreNotEqualsIfClassDefIsDifferent()
        {
            var def1 = Create();
            def1.ClassDef = new ClassDefinition { Name = "class def", Modifier = "with modifier" };
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreNotEqualsIfMessagesIsDifferent()
        {
            var def1 = Create();
            def1.Messages = new List<string> { "msg3" };
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreNotEqualsIfLeftMessagesIsNull()
        {
            var def1 = Create();
            def1.Messages = null;
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreNotEqualsIfRightMessagesIsNull()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Messages = null;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreNotEqualsIfDiagnosticDefIsDifferent()
        {
            var def1 = Create();
            def1.DiagnosticDef = new DiagnosticDefinition { Parameter = "another" };
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreNotEqualsIfDiagnosticDefIsDifferentBlank()
        {
            var def1 = Create();
            def1.DiagnosticDef = new DiagnosticDefinition();
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetDiagnosticSetTheDiagnostic()
        {
            var def = new ExceptionClassDefinition();
            def.SetDiagnostic(new DiagnosticDefinition { Parameter = "My param" });

            Assert.Equal("My param",
                         def.DiagnosticDef.Parameter);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetDiagnosticDoesNotSetTheDiagnosticIfItIsAlreadyInitialized()
        {
            var def = new ExceptionClassDefinition();
            def.SetDiagnostic(new DiagnosticDefinition { Parameter = "My param", Initialized = true });
            def.SetDiagnostic(new DiagnosticDefinition { Parameter = "My other param" });

            Assert.Equal("My param",
                         def.DiagnosticDef.Parameter);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetDiagnosticSetTheDiagnosticIfNotInitialized()
        {
            var def = new ExceptionClassDefinition();
            def.SetDiagnostic(new DiagnosticDefinition { Parameter = "My param", Initialized = false });
            def.SetDiagnostic(new DiagnosticDefinition { Parameter = "My other param" });

            Assert.Equal("My other param",
                         def.DiagnosticDef.Parameter);
        }
    }
}
