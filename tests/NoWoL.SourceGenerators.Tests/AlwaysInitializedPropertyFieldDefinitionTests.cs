using NoWoL.SourceGenerators.AlwaysInitializedProperty;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{
    public class AlwaysInitializedPropertyFieldDefinitionTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void BlanksAreEquals()
        {
            var def1 = new AlwaysInitializedPropertyFieldDefinition();
            var def2 = new AlwaysInitializedPropertyFieldDefinition();

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
            var def1 = new AlwaysInitializedPropertyFieldDefinition();
            var def2 = new AlwaysInitializedPropertyFieldDefinition();

            Assert.True(def1.Equals((object?)def2));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsObjectsReturnsFalseForDiffType()
        {
            var def1 = new AlwaysInitializedPropertyFieldDefinition();
            var def2 = new object();

            Assert.False(def1.Equals((object?)def2));
        }

        private AlwaysInitializedPropertyFieldDefinition Create()
        {
            var def = new AlwaysInitializedPropertyFieldDefinition
                      {
                          Name = "Freddie",
                          Namespace = "ns",
                          ClassDef = new ClassDefinition { Name = "Mercury" },
                          DiagnosticDef = new DiagnosticDefinition { Parameter = "another" },
                          LeadingTrivia = "trivia",
                          ParentClasses = new List<ClassDefinition> { new ClassDefinition { Name = "p1" }, new ClassDefinition { Name = "p2" } },
                          Type = "SomeType",
                          UsingStatements = new List<string>
                                            {
                                                "using1",
                                                "using2"
                                            }.ToImmutableArray()
                      };

            return def;
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsWithEveryPropertiesSame()
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
        public void NotEqualsBecauseNamespaceIsDifferent()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Namespace = "another ns";

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseLeftNamespaceIsNull()
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
        public void NotEqualsBecauseRightNamespaceIsNull()
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
        public void NotEqualsBecauseParentClassesIsDifferent()
        {
            var def1 = Create();
            var def2 = Create();
            def2.ParentClasses = new List<ClassDefinition> { new ClassDefinition { Name = "p3" } };

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseLeftParentClassesIsNull()
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
        public void NotEqualsBecauseRightParentClassesIsNull()
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
        public void NotEqualsBecauseClassDefIsDifferent()
        {
            var def1 = Create();
            var def2 = Create();
            def2.ClassDef = new ClassDefinition { Modifier = "different" };

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseUsingStatementsIsDifferent()
        {
            var def1 = Create();
            var def2 = Create();
            def2.UsingStatements = new List<string> { "u" }.ToImmutableArray();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseLeftUsingStatementsIsDefault()
        {
            var def1 = Create();
            def1.UsingStatements = default;
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseRightUsingStatementsIsDefault()
        {
            var def1 = Create();
            var def2 = Create();
            def2.UsingStatements = default;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseLeftUsingStatementsIsEmpty()
        {
            var def1 = Create();
            def1.UsingStatements = new List<string>().ToImmutableArray();
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseRightUsingStatementsIsEmpty()
        {
            var def1 = Create();
            var def2 = Create();
            def2.UsingStatements = new List<string>().ToImmutableArray();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseNameIsDifferent()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Name = "another";

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseLeftNameIsNull()
        {
            var def1 = Create();
            def1.Name = null;
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseRightNameIsNull()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Name = null;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseTypeIsDifferent()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Type = "another ns";

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseLeftTypeIsNull()
        {
            var def1 = Create();
            def1.Type = null;
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseRightTypeIsNull()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Type = null;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseLeadingTriviaIsDifferent()
        {
            var def1 = Create();
            var def2 = Create();
            def2.LeadingTrivia = "another ns";

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseLeftLeadingTriviaIsNull()
        {
            var def1 = Create();
            def1.LeadingTrivia = null;
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseRightLeadingTriviaIsNull()
        {
            var def1 = Create();
            var def2 = Create();
            def2.LeadingTrivia = null;

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseDiagnosticDefIsDifferent()
        {
            var def1 = Create();
            var def2 = Create();
            def2.DiagnosticDef = new DiagnosticDefinition { Initialized = true, Parameter = "p" };

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseLeftDiagnosticDefIsDefault()
        {
            var def1 = Create();
            def1.DiagnosticDef = default;
            var def2 = Create();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseRightDiagnosticDefIsDefault()
        {
            var def1 = Create();
            var def2 = Create();
            def2.DiagnosticDef = default;

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
            var def = new AlwaysInitializedPropertyFieldDefinition();
            def.SetDiagnostic(new DiagnosticDefinition { Parameter = "My param" });

            Assert.Equal("My param",
                         def.DiagnosticDef.Parameter);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetDiagnosticDoesNotSetTheDiagnosticIfItIsAlreadyInitialized()
        {
            var def = new AlwaysInitializedPropertyFieldDefinition();
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
            var def = new AlwaysInitializedPropertyFieldDefinition();
            def.SetDiagnostic(new DiagnosticDefinition { Parameter = "My param", Initialized = false });
            def.SetDiagnostic(new DiagnosticDefinition { Parameter = "My other param" });

            Assert.Equal("My other param",
                         def.DiagnosticDef.Parameter);
        }
    }
}
