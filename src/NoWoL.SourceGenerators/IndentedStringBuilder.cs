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

        public IndentedStringBuilder Add(string? text,
                                         bool addNewLine = false,
                                         bool removeLastNewLines = false)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (addNewLine)
                {
                    _builder.AppendLine(String.Empty);
                }

                return this;
            }

            var textLength = text!.Length;

            if (removeLastNewLines)
            {
                for (var i = textLength - 1; CCC(i, text) ; i--)
                {
                    textLength--;
                }
            }

            _builder.EnsureCapacity(_builder.Capacity + textLength);

            var start = 0;
            for (var i = 0; i < textLength; i++)
            {
                if (text[i] == '\r')
                {
                    int length = i - start;
                    if (i + 1 < textLength && text[i + 1] == '\n')
                    {
                        i++;
                    }

                    Append(text.AsSpan(start,
                                       length));

                    if (addNewLine)
                    {
                        _builder.Append(Environment.NewLine);
                    }

                    start = i + 1;
                }
                else if (text[i] == '\n')
                {
                    int length = i - start;

                    Append(text.AsSpan(start,
                                    length));

                    if (addNewLine)
                    {
                        _builder.Append(Environment.NewLine);
                    }

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
                    Append(text.AsSpan(start,
                                       textLength - start));

                    if (addNewLine)
                    {
                        _builder.Append(Environment.NewLine);
                    }
                }
            }

            return this;
        }

        private bool CCC(int i, string text)
        {
            if (i >= 0)
            {
                return IsNewLineCharacter(text[i]);
            }

            return false;
        }

        private static bool IsNewLineCharacter(char c)
        {
            if (c == '\r')
            {
                return true;
            }

            if (c == '\n')
            {
                return true;
            }

            return false;
        }

        private void AppendIndent()
        {
            for (var i = 0; i < Indent; i++)
            {
                _builder.Append("    ");
            }
        }

        public void AddRaw(string? value)
        {
            _builder.Append(value);
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

        public void Clear(bool resetIndent)
        {
            _builder.Clear();

            if (resetIndent)
            {
                Indent = 0;
            }
        }
    }
}
