using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{
    public class ClassDefinitionTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void BlanksAreEquals()
        {
            var def1 = new ClassDefinition();
            var def2 = new ClassDefinition();

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
            var def1 = new ClassDefinition();
            var def2 = new ClassDefinition();

            Assert.True(def1.Equals((object?)def2));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EqualsObjectsReturnsFalseForDiffType()
        {
            var def1 = new ClassDefinition();
            var def2 = new object();

            Assert.False(def1.Equals((object?)def2));
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData("name", "name", "modifier", "modifier")]
        [InlineData("name", "name", "", "")]
        [InlineData("name", "name", null, null)]
        [InlineData("", "", "modifier", "modifier")]
        [InlineData(null, null, "modifier", "modifier")]
        [InlineData("", "", "", "")]
        [InlineData(null, null, null, null)]
        public void DefinitionWithSameValueGenerateSameHashcodeAndAreEqual(string def1Name, string def2Name, string def1Mod, string def2Mod)
        {
            var def1 = new ClassDefinition { Name = def1Name, Modifier = def1Mod };
            var def2 = new ClassDefinition { Name = def2Name, Modifier = def2Mod };

            Assert.Equal(def1.GetHashCode(),
                         def2.GetHashCode());

            Assert.Equal(def1,
                         def2);
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData("name", "name1", "modifier", "modifier")]
        [InlineData("name", "name", "modifier", "")]
        [InlineData("name", "name", null, "")]
        [InlineData("", "a", "modifier", "modifier")]
        [InlineData(null, "name", "modifier", "modifier")]
        [InlineData("", "", "mod", "")]
        [InlineData(null, null, "", null)]
        public void DefinitionWithDifferentValueGenerateSameHashcodeAndAreEqual(string def1Name, string def2Name, string def1Mod, string def2Mod)
        {
            var def1 = new ClassDefinition { Name = def1Name, Modifier = def1Mod };
            var def2 = new ClassDefinition { Name = def2Name, Modifier = def2Mod };

            Assert.NotEqual(def1.GetHashCode(),
                            def2.GetHashCode());

            Assert.NotEqual(def1,
                            def2);
        }
    }
}
