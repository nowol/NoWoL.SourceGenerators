using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{
    public class GenerationHelpers
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void ConvertErrorCodeShouldThrowForInvalidValue()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SourceGenerators.GenerationHelpers.ConvertErrorCode((ErrorCode)66666));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsPartialClassReturnsFalseIfSyntaxIsNull()
        {
            Assert.False(SourceGenerators.GenerationHelpers.IsPartialClass(null));
        }
    }
}