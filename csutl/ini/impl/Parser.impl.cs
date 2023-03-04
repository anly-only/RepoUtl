using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace csutl.ini.impl;

class Parser
{
    public Format Format { get; }

    internal Parser(Format f)
    {
        this.Format = f;
    }
    internal void ParseIniBody(string body,
        Action<string, string> OnSection,
        Action<string, string, string> OnItem)
    {
        OnSection("", "");
        var it = new IT(body);
        while (!it.IsEof())
        {
            it.SkipSpaces();
            if (it.Current == '[') // section
            {
                it.Next();
                string section = this.ParseText(it, "]");
                string comment = string.Empty;
                Debug.Assert(it.Current == ']');
                it.Next();
                it.SkipSpaces(/*true*/);
                if (it.IsEqual("//"))
                {
                    it.Next();
                    it.Next();
                    comment = this.ParseText(it);
                }
                if (!it.IsEolEof())
                    throw new Exception();
                it.SkipEol();

                OnSection(section, comment);
            }
            else // item
            {
                string key = string.Empty, value, comment = string.Empty;
                var s = this.ParseText(it, "=", "//");
                if (it.IsEqual("="))
                {
                    key = s;
                    it.Next();
                    value = this.ParseText(it, "//");
                }
                else
                    value = s;

                if (it.IsEqual("//"))
                {
                    it.Next();
                    it.Next();
                    comment = this.ParseText(it);
                }

                if (!it.IsEolEof())
                    throw new Exception();
                it.SkipEol();

                OnItem(key, value, comment);
            }
        }
    }

    string ParseText(IT it, params string[] stop)
    {
        return SB.BuildString(sb =>
        {
            it.SkipSpaces(/*true*/);
            int start = it.Position;
            bool loop = true;
            while (loop)
            {
                if (it.Current == this.Format.Quote)
                {
                    sb.Append(it.Range(start, it.Position, false));
                    it.Next();
                    this.ParseQText(sb, it);
                    start = it.Position;
                }
                else if (it.IsEqualAny(stop) || it.IsEolEof())
                {
                    sb.Append(it.Range(start, it.Position, true));
                    loop = false;
                }
                else
                {
                    it.Next();
                }
            }
            return sb.ToString();
        });
    }

    void ParseQText(StringBuilder sb, IT it)
    {
        int start = it.Position;
        bool loop = true;
        while (loop)
        {
            if (it.Current == this.Format.Quote)
            {
                sb.Append(it.Range(start, it.Position, false));
                it.Next();
                if (it.Current == this.Format.Quote)
                {
                    sb.Append(this.Format.Quote);
                    it.Next();
                    start = it.Position;
                }
                else
                {
                    loop = false;
                }
            }
            else if (it.IsEof())
                throw new Exception();
            else
                it.Next();
        }
        Debug.Assert(it.GetCurrent(-1) == this.Format.Quote);
    }


    [DebuggerDisplay("{DebuggerDisplay}")]
    class IT
    {
        string text;
        int position;

        internal IT(string text, int pos = 0)
        {
            this.text = text;
            this.position = pos;
        }

        internal void Next()
        {
            if (this.position < this.text.Length)
                this.position++;
        }

        internal bool IsEqual(string value, int pos = 0) => this.text.IsEqual(this.position + pos, value);
        internal bool IsEqualAny(IEnumerable<string> values, int pos = 0)
        {
            foreach (var value in values)
            {
                if (this.IsEqual(value, pos))
                    return true;
            }
            return false;
        }

        internal char Current => this.GetCurrent(0);

        internal char GetCurrent(int offset)
        {
            int pos = this.position + offset;
            return pos < this.text.Length && pos >= 0 ? this.text[pos] : (char)0;
        }

        internal string Text => this.text;
        internal string DebuggerDisplay => this.text.Substring(this.Position);
        internal int Position => this.position;

        internal bool IsEol(int pos = 0) => this.GetCurrent(pos) == '\r' || this.GetCurrent(pos) == '\n';
        internal bool IsEof(int pos = 0) => this.GetCurrent(pos) == 0;
        internal bool IsEolEof(int pos = 0) => this.IsEol(pos) || this.IsEof(pos);


        internal void SkipSpaces(/*bool skipContinuator = false*/)
        {
            bool loop = true;
            while (loop)
            {
                if (this.Current == ' ' || this.Current == '\t')
                    this.Next();
                else
                    loop = false;
            }
        }

        internal void SkipEol()
        {
            if (this.Current == '\r')
                this.Next();
            if (this.Current == '\n')
                this.Next();
        }

        internal ReadOnlySpan<char> Range(int begin, int end, bool trimEnd)
        {
            var s = this.Text.AsSpan().Slice(begin, end - begin);
            if (trimEnd)
                s = s.TrimEnd();
            return s;
        }
    }
}

