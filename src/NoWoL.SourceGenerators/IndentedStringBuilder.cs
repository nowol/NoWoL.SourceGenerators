using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NoWoL.SourceGenerators
{
    internal class IndentedStringBuilder
    {
        private readonly StringBuilder _builder = new();
        public int Indent { get; private set; } = 0;

        public override string ToString()
        {
            return _builder.ToString();
        }

        public int Length
        {
            get
            {
                return _builder.Length;
            }
        }

        public IndentedStringBuilder IncreaseIndent()
        {
            Indent++;

            return this;
        }

        public IndentedStringBuilder DecreaseIndent()
        {
            if (Indent > 0)
            {
                Indent--;
            }

            return this;
        }

        private IndentedStringBuilder Append(string text, bool skipIndent = false)
        {
            if (!skipIndent)
            {
                AppendIndent();
            }

            if (String.IsNullOrWhiteSpace(text))
            {
                return this;
            }

            _builder.Append(text);

            return this;
        }

        private void AppendIndent()
        {
            for (var i = 0; i < Indent; i++)
            {
                _builder.Append("    ");
            }
        }

        public IndentedStringBuilder AppendLine(string text, bool skipIndent = false)
        {
            Append(text, skipIndent: skipIndent);
            _builder.Append(Environment.NewLine);

            return this;
        }

        public IndentedStringBuilder AppendLine(ReadOnlySpan<char> value)
        {
            Append(value);

            _builder.Append(Environment.NewLine);

            return this;
        }

        private void Append(ReadOnlySpan<char> value)
        {
            if (!value.IsEmpty)
            {
                AppendIndent();

                foreach (var c in value)
                {
                    _builder.Append(c);
                }
            }
        }

        public IndentedStringBuilder AppendLines(string text, bool removeLastNewLines = false)
        {
            if (text == null)
            {
                return AppendLine(String.Empty);
            }

            var textLength = text.Length;

            if (removeLastNewLines)
            {
                for (var i = textLength - 1; i >= 0; i--)
                {
                    if (text[i] == '\r'
                        || text[i] == '\n')
                    {
                        textLength--;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            _builder.EnsureCapacity(_builder.Capacity + textLength);

            var start = 0;
            for (var i = 0; i < textLength; i++)
            {
                if (text[i] == '\r')
                {
                    int length = i - start;
                    if (i + 1 < textLength && text[i+1] == '\n')
                    {
                        i++;
                    }

                    AppendLine(text.AsSpan(start,
                                           length));
                    start = i + 1;
                }
                else if (text[i] == '\n')
                {
                    int length = i - start;

                    AppendLine(text.AsSpan(start,
                                           length));
                    start = i + 1;
                }
            }

            if (start < textLength)
            {
                if (removeLastNewLines)
                {
                    Append(text.AsSpan(start,
                                       textLength - start));
                }
                else
                {
                    AppendLine(text.AsSpan(start,
                                           textLength - start));
                }
            }

            return this;
        }
    }
}
