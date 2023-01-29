using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{
    public class ComparisonHelpersTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreEquatableReturnsTrueIfBothNull()
        {
            Assert.True(ComparisonHelpers.AreEquatable<EquatableTestClass>(null, null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreEquatableReturnsFalseIfRightIsNull()
        {
            var c1 = new EquatableTestClass();
            Assert.False(ComparisonHelpers.AreEquatable(c1, null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreEquatableReturnsFalseIfLeftIsNull()
        {
            var c1 = new EquatableTestClass();
            Assert.False(ComparisonHelpers.AreEquatable(null, c1));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreEquatableReturnsTrueIfBothEquals()
        {
            var c1 = new EquatableTestClass();
            var c2 = new EquatableTestClass();
            Assert.True(ComparisonHelpers.AreEquatable(c1, c2));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreEquatableReturnsFalseIfDifferent()
        {
            var c1 = new EquatableTestClass { Name = "Freddie" };
            var c2 = new EquatableTestClass();
            Assert.False(ComparisonHelpers.AreEquatable(c1, c2));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void BothNullReturnsTrueIfBothNull()
        {
            Assert.True(ComparisonHelpers.BothNull<EquatableTestClass>(null, null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void BothNullReturnsFalseIfLeftIsNull()
        {
            var c1 = new EquatableTestClass();
            Assert.False(ComparisonHelpers.BothNull<EquatableTestClass>(null, c1));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void BothNullReturnsFalseIfRightIsNull()
        {
            var c1 = new EquatableTestClass();
            Assert.False(ComparisonHelpers.BothNull<EquatableTestClass>(c1, null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreCollectionEqualsReturnsTrueIfBothNull()
        {
            Assert.True(ComparisonHelpers.AreCollectionEquals<EquatableTestClass>(null, null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreCollectionEqualsReturnsTrueIfListContainsSameElements()
        {
            var col1 = new List<EquatableTestClass> { new EquatableTestClass(), new EquatableTestClass { Name = "Freddie" } };
            var col2 = new List<EquatableTestClass> { new EquatableTestClass(), new EquatableTestClass { Name = "Freddie" } };
            Assert.True(ComparisonHelpers.AreCollectionEquals<EquatableTestClass>(col1, col2));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreCollectionEqualsReturnsFalseIfListContainsDifferentNumberOfElements()
        {
            var col1 = new List<EquatableTestClass> { new EquatableTestClass(), new EquatableTestClass { Name = "Freddie" } };
            var col2 = new List<EquatableTestClass> { new EquatableTestClass() };
            Assert.False(ComparisonHelpers.AreCollectionEquals<EquatableTestClass>(col1, col2));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreCollectionEqualsReturnsFalseIfLeftIsNull()
        {
            var col2 = new List<EquatableTestClass> { new EquatableTestClass() };
            Assert.False(ComparisonHelpers.AreCollectionEquals<EquatableTestClass>(null, col2));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreCollectionEqualsReturnsFalseIfRightIsNull()
        {
            var col1 = new List<EquatableTestClass> { new EquatableTestClass(), new EquatableTestClass { Name = "Freddie" } };
            Assert.False(ComparisonHelpers.AreCollectionEquals<EquatableTestClass>(col1, null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AreCollectionEqualsReturnsTrueIfListContainsDiffElements()
        {
            var col1 = new List<EquatableTestClass> { new EquatableTestClass(), new EquatableTestClass { Name = "Freddie" } };
            var col2 = new List<EquatableTestClass> { new EquatableTestClass(), new EquatableTestClass { Name = "Mercury" } };
            Assert.False(ComparisonHelpers.AreCollectionEquals<EquatableTestClass>(col1, col2));
        }

        private class EquatableTestClass : IEquatable<EquatableTestClass>
        {
            public string? Name { get; set; }

            public bool Equals(EquatableTestClass? other)
            {
                if (ReferenceEquals(null,
                                    other))
                {
                    return false;
                }

                if (ReferenceEquals(this,
                                    other))
                {
                    return true;
                }

                return Name == other.Name;
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null,
                                    obj))
                {
                    return false;
                }

                if (ReferenceEquals(this,
                                    obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return Equals((EquatableTestClass)obj);
            }

            public override int GetHashCode()
            {
                return Name?.GetHashCode() ?? 0;
            }
        }
    }
}
