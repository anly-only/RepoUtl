using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csutl.ini.impl;

class FormatImpl : Format
{
    public char Quote { get; set; } = '\'';

    #region Save
    public Quotes Quotes { get; set; }
    public bool BlankLineAfterSection { get; set; } = true;
    public int ItemIndent { get; set; } = 3;
    public int SubItemIndent { get; set; } = 6;
    public int CommentAlignMaxPos { get; set; } = 50; // 0 - disable comment align
    #endregion

    internal FormatImpl()
    {
    }

    // if commentAlign < 0 format without comment
    internal static void FormatSection(StringBuilder sb, Section sec, int commentAlign)
    {
        int begin = sb.Length;
        sb.Append("[");
        if (sec.IsName)
            InQuotes(sb, sec.Name, sec.ini.Format.Quote, false, "[");
        sb.Append("]");
        if (sec.IsComment && commentAlign >= 0)
        {
            if (commentAlign > 0)
            {
                int x = commentAlign - (sb.Length - begin) - 2;
                if (x > 0)
                    sb.Append(' ', x);
            }
            sb.Append(" // ");
            InQuotes(sb, sec.Comment, sec.ini.Format.Quote, false, "//");
        }
    }

    // if commentAlign < 0 format without comment
    internal static void FormatItem(StringBuilder sb, ItemImpl item, int indent, int commentAlign)
    {
        int begin = sb.Length;
        var f = item.Section.ini.Format;
        if (indent > 0 && !item.IsEmpty)
            sb.Append(' ', indent);

        int start = sb.Length;

        if (item.IsKey)
        {
            bool x = item.Key.FirstOrDefault() == '[';
            InQuotes(sb, item.Key, f.Quote, x, "=", "//"); // [ as first !!!
            sb.Append(" = ");
        }

        if (item.IsValue)
        {
            bool x = start == sb.Length && item.Value.FirstOrDefault() == '[';
            InQuotes(sb, item.Value, f.Quote, x, "//"); // [ as first !!! 
        }

        if (item.IsComment && commentAlign >= 0)
        {
            if (sb.Length != start)
                sb.Append(' ');

            if (item.IsKey || item.IsValue) // item without key and value allign as item, not as comment
            {
                if (commentAlign > 0)
                {
                    int x = commentAlign - (sb.Length - begin) - 1;
                    if (x > 0)
                        sb.Append(' ', x);
                }
            }
            sb.Append("// ");
            InQuotes(sb, item.Comment, f.Quote, false, "//");
        }
    }

    static void InQuotes(StringBuilder sb, string text, char quote, bool force, params string[] separators)
    {
        InQuotes(sb, text, quote, force, (IEnumerable<string>)separators);
    }

    static void InQuotes(StringBuilder sb, string text, char quote, bool force, IEnumerable<string> separators)
    {
        if (text.Length != 0)
        {
            int n = text.Length;
            var q = quote.ToString();
            text = text.Replace(q, q + q);
            if (force || text.Length != n || text[0] == ' ' || text.LastChar() == ' ' || text.IndexOfAny(separators.Append("\n")) != -1)
            {
                sb.Append(q);
                sb.Append(text);
                sb.Append(q);
            }
            else
            {
                sb.Append(text);
            }
        }
    }

    internal int CalculateCommentAllign(Section section)
    {
        var e = section.GetText(true); // without comments
        int max = e.Any()
            ? e.Max(a => Math.Min(a.Length, this.CommentAlignMaxPos)) + 2
            : 0;
        return max;
    }
}


