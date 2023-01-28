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
    public class ClassWithParentAndNamespaceDefinitionTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void BlanksAreEquals()
        {
            var def1 = new ClassWithParentAndNamespaceDefinition();
            var def2 = new ClassWithParentAndNamespaceDefinition();

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
            var def1 = new ClassWithParentAndNamespaceDefinition();
            var def2 = new ClassWithParentAndNamespaceDefinition();

            Assert.True(def1.Equals((object?)def2));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsObjectsReturnsFalseForDiffType()
        {
            var def1 = new ClassWithParentAndNamespaceDefinition();
            var def2 = new object();

            Assert.False(def1.Equals((object?)def2));
        }

        private ClassWithParentAndNamespaceDefinition Create()
        {
            var def = new ClassWithParentAndNamespaceDefinition
            {
                Namespace = "ns",
                ClassDef = new ClassDefinition { Name = "Mercury" },
                ParentClasses = new List<ClassDefinition> { new ClassDefinition { Name = "p1" }, new ClassDefinition { Name = "p2" } },
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
    }
}
