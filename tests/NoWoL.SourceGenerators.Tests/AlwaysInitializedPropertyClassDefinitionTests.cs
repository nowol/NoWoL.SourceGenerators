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
    public class AlwaysInitializedPropertyClassDefinitionTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void BlanksAreEquals()
        {
            var def1 = new AlwaysInitializedPropertyClassDefinition();
            var def2 = new AlwaysInitializedPropertyClassDefinition();

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
            var def1 = new AlwaysInitializedPropertyClassDefinition();
            var def2 = new AlwaysInitializedPropertyClassDefinition();

            Assert.True(def1.Equals((object?)def2));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsObjectsReturnsFalseForDiffType()
        {
            var def1 = new AlwaysInitializedPropertyClassDefinition();
            var def2 = new object();

            Assert.False(def1.Equals((object?)def2));
        }

        private AlwaysInitializedPropertyClassDefinition Create()
        {
            var def = new AlwaysInitializedPropertyClassDefinition
                      {
                          AdvClassDef = new ClassWithParentAndNamespaceDefinition { Namespace = "ns" },
                          Fields = new List<AlwaysInitializedPropertyFieldDefinition>
                                   {
                                       new AlwaysInitializedPropertyFieldDefinition { Name = "field1" },
                                       new AlwaysInitializedPropertyFieldDefinition { Name = "field2" }
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
        public void NotEqualsBecauseAdvClassDefIsDifferent()
        {
            var def1 = Create();
            var def2 = Create();
            def2.AdvClassDef = new ClassWithParentAndNamespaceDefinition { Namespace = "another ns" };

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEqualsBecauseFieldsDifferent()
        {
            var def1 = Create();
            var def2 = Create();
            def2.Fields = new List<AlwaysInitializedPropertyFieldDefinition>
                          {
                              new AlwaysInitializedPropertyFieldDefinition { Name = "field3" },
                              new AlwaysInitializedPropertyFieldDefinition { Name = "field2" }
                          }.ToImmutableArray();

            Assert.NotEqual(def1,
                            def2);

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());
        }
    }
}
