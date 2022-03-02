using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NoWoL.SourceGenerators.Tests
{
    public class IndentedStringBuilderTests
    {
        private readonly IndentedStringBuilder _sut = new();

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AddingTextIncreaseTheLength()
        {
            _sut.AppendLine("abc").AppendLine("def");
            Assert.Equal($"abc{Environment.NewLine}def{Environment.NewLine}".Length,
                         _sut.Length);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendTwoLines()
        {
            _sut.AppendLine("abc").AppendLine("def");
            Assert.Equal($"abc{Environment.NewLine}def{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendTwoLinesWithIndentOfOne()
        {
            _sut.IncreaseIndent().AppendLine("abc").AppendLine("def");
            Assert.Equal($"    abc{Environment.NewLine}    def{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendThreeLines()
        {
            _sut.AppendLine("abc").AppendLine("def").AppendLine("ghi");
            Assert.Equal($"abc{Environment.NewLine}def{Environment.NewLine}ghi{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendThreeLinesWithIndentOfOne()
        {
            _sut.IncreaseIndent().AppendLine("abc").AppendLine("def").AppendLine("ghi");
            Assert.Equal($"    abc{Environment.NewLine}    def{Environment.NewLine}    ghi{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendLineWithMultipleIndent()
        {
            _sut.IncreaseIndent().AppendLine("abc").IncreaseIndent().AppendLine("def").DecreaseIndent().AppendLine("aaa");
            Assert.Equal($"    abc{Environment.NewLine}        def{Environment.NewLine}    aaa{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IncreaseIndentFollowedByDecreaseIndentCancelEachOtherOut()
        {
            _sut.IncreaseIndent().DecreaseIndent().AppendLine("abc").AppendLine("def");
            Assert.Equal($"abc{Environment.NewLine}def{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void DecreaseIndentWithoutIncreaseIndentDoesNothing()
        {
            _sut.DecreaseIndent().AppendLine("abc");
            Assert.Equal($"abc{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendMultipleLinesAtOnce()
        {
            _sut.AppendLines("abc\r\ndef\r\n");
            Assert.Equal($"abc{Environment.NewLine}def{Environment.NewLine}",
                         _sut.ToString());
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public void AppendLinesWithOnlyOneNewLine(string value)
        {
            _sut.AppendLines(value);
            Assert.Equal($"{Environment.NewLine}",
                         _sut.ToString());
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData("\n\n")]
        [InlineData("\r\n\r\n")]
        [InlineData("\n\r")]
        public void AppendLinesWithTwoNewLine(string value)
        {
            _sut.AppendLines(value);
            Assert.Equal($"{Environment.NewLine}{Environment.NewLine}",
                         _sut.ToString());
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData("\n\n")]
        [InlineData("\r\n\r\n")]
        [InlineData("\n\r")]
        public void AppendLinesWithOnlyNewLinesShouldNotIndent(string value)
        {
            _sut.IncreaseIndent().IncreaseIndent().AppendLines(value);
            Assert.Equal($"{Environment.NewLine}{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AddingMultipleLineTextWithoutIndent()
        {
            _sut.AppendLines("hello\nthere\r  how  \r\nyoudoing");
            Assert.Equal($"hello{Environment.NewLine}there{Environment.NewLine}  how  {Environment.NewLine}youdoing{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AddingMultipleLineTextWithIndent()
        {
            _sut.IncreaseIndent().AppendLines("hello\nthere\r  how  \r\nyoudoing");
            Assert.Equal($"    hello{Environment.NewLine}    there{Environment.NewLine}      how  {Environment.NewLine}    youdoing{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendLinesWithNullTextGivesNewLineOnlyWithoutIndent()
        {
            _sut.AppendLines(null);
            Assert.Equal($"{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendLinesWithNullTextGivesNewLineOnlyWithIndent()
        {
            _sut.IncreaseIndent().AppendLines(null);
            Assert.Equal($"    {Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendLinesWithEmptyTextGivesNewLineOnlyWithoutIndent()
        {
            _sut.AppendLines(null);
            Assert.Equal($"{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendLinesWithEmptyTextGivesNewLineOnlyWithIndent()
        {
            _sut.IncreaseIndent().AppendLines(null);
            Assert.Equal($"    {Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendLinesShouldNotAddTheLastNewLine()
        {
            _sut.IncreaseIndent().AppendLines("abc\ndef", removeLastNewLines: true);
            Assert.Equal($"    abc{Environment.NewLine}    def",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendLinesShouldRemoveTheLastNewLine()
        {
            _sut.IncreaseIndent().AppendLines("abc\ndef\n", removeLastNewLines: true);
            Assert.Equal($"    abc{Environment.NewLine}    def",
                         _sut.ToString());
        }
    }
}
