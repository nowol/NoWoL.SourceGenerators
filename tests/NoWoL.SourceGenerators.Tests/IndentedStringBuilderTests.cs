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
        public void AddingTextDoesNotAddNewLine()
        {
            _sut.Add("abc").Add("def");
            Assert.Equal("abcdef",
                         _sut.ToString());
        }
        [Fact]
        [Trait("Category",
               "Unit")]
        public void AddingTextIncreaseTheLength()
        {
            _sut.Add("abc", addNewLine: true).Add("def");
            Assert.Equal($"abc{Environment.NewLine}def".Length,
                         _sut.Length);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendTwoLines()
        {
            _sut.Add("abc", addNewLine: true).Add("def", addNewLine: true);
            Assert.Equal($"abc{Environment.NewLine}def{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendTwoLinesWithIndentOfOne()
        {
            _sut.IncreaseIndent().Add("abc", addNewLine: true).Add("def", addNewLine: true);
            Assert.Equal($"    abc{Environment.NewLine}    def{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendThreeLines()
        {
            _sut.Add("abc", addNewLine: true).Add("def", addNewLine: true).Add("ghi", addNewLine: true);
            Assert.Equal($"abc{Environment.NewLine}def{Environment.NewLine}ghi{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendThreeLinesWithIndentOfOne()
        {
            _sut.IncreaseIndent().Add("abc", addNewLine: true).Add("def", addNewLine: true).Add("ghi", addNewLine: true);
            Assert.Equal($"    abc{Environment.NewLine}    def{Environment.NewLine}    ghi{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendLineWithMultipleIndent()
        {
            _sut.IncreaseIndent().Add("abc", addNewLine: true).IncreaseIndent().Add("def", addNewLine: true).DecreaseIndent().Add("aaa", addNewLine: true);
            Assert.Equal($"    abc{Environment.NewLine}        def{Environment.NewLine}    aaa{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IncreaseIndentFollowedByDecreaseIndentCancelEachOtherOut()
        {
            _sut.IncreaseIndent().DecreaseIndent().Add("abc", addNewLine: true).Add("def", addNewLine: true);
            Assert.Equal($"abc{Environment.NewLine}def{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void DecreaseIndentWithoutIncreaseIndentDoesNothing()
        {
            _sut.DecreaseIndent().Add("abc", addNewLine: true);
            Assert.Equal($"abc{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendMultipleLinesAtOnce()
        {
            _sut.Add("abc\r\ndef\r\n", addNewLine: true);
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
            _sut.Add(value, addNewLine: true);
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
            _sut.Add(value, addNewLine: true);
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
            _sut.IncreaseIndent().IncreaseIndent().Add(value, addNewLine: true);
            Assert.Equal($"{Environment.NewLine}{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AddingMultipleLineTextWithoutIndent()
        {
            _sut.Add("hello\nthere\r  how  \r\nyoudoing", addNewLine: true);
            Assert.Equal($"hello{Environment.NewLine}there{Environment.NewLine}  how  {Environment.NewLine}youdoing{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AddingMultipleLineTextWithIndent()
        {
            _sut.IncreaseIndent().Add("hello\nthere\r  how  \r\nyoudoing", addNewLine: true);
            Assert.Equal($"    hello{Environment.NewLine}    there{Environment.NewLine}      how  {Environment.NewLine}    youdoing{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendLinesWithNullTextGivesNewLineOnlyWithoutIndent()
        {
            _sut.Add(null, addNewLine: true);
            Assert.Equal($"{Environment.NewLine}",
                         _sut.ToString());
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData((string?)null)]
        [InlineData("")]
        public void AppendLinesWithEmptyTextGivesNewLineOnlyWithIndent(string? text)
        {
            _sut.IncreaseIndent().Add(text, addNewLine: true);
            Assert.Equal($"{Environment.NewLine}",
                         _sut.ToString());
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData((string?)null)]
        [InlineData("")]
        public void AppendLinesWithEmptyTextGivesNewLineOnlyWithoutIndent(string? text)
        {
            _sut.Add(text, addNewLine: true);
            Assert.Equal($"{Environment.NewLine}",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendLinesShouldNotAddTheLastNewLine()
        {
            _sut.IncreaseIndent().Add("abc\ndef", removeLastNewLines: true, addNewLine: true);
            Assert.Equal($"    abc{Environment.NewLine}    def",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendLinesShouldRemoveTheMacNewLinesAtTheEnd()
        {
            _sut.IncreaseIndent().Add("abc\rdef\r", removeLastNewLines: true, addNewLine: true);
            Assert.Equal($"    abc{Environment.NewLine}    def",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AddingAnEmptyString()
        {
            _sut.IncreaseIndent().Add("\r", removeLastNewLines: true, addNewLine: true);
            Assert.Equal($"",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AppendLinesShouldRemoveTheLastNewLine()
        {
            _sut.IncreaseIndent().Add("abc\ndef\n", removeLastNewLines: true, addNewLine: true);
            Assert.Equal($"    abc{Environment.NewLine}    def",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ClearShouldRemoveTheContentAndKeepIndent()
        {
            _sut.IncreaseIndent().Add("abc");
            Assert.Equal("    abc",
                         _sut.ToString());
            
            _sut.Clear(false);

            Assert.Equal(1,
                         _sut.Indent);

            Assert.Equal(String.Empty,
                         _sut.ToString());
            _sut.Add("def");

            Assert.Equal("    def",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ClearShouldRemoveTheContentAndResetIndent()
        {
            _sut.IncreaseIndent().Add("abc");
            Assert.Equal("    abc",
                         _sut.ToString());
            
            _sut.Clear(true);

            Assert.Equal(0,
                         _sut.Indent);

            Assert.Equal(String.Empty,
                         _sut.ToString());
            _sut.Add("def");

            Assert.Equal("def",
                         _sut.ToString());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AddWithoutNewLineShouldNotIndent()
        {
            _sut.IncreaseIndent().Add("abc").Add("def");
            Assert.Equal("    abcdef",
                         _sut.ToString());

            // todo fix ISB
        }
    }
}
