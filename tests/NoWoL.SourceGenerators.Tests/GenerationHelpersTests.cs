using Microsoft.CodeAnalysis.Testing;
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
    }
}