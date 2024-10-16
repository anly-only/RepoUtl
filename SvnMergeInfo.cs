using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RepoUtl
{
    internal class SvnMergeInfo
    {
        class Range
        {
            internal int Begin { get; }
            internal int End { get; }

            internal Range(int begin)
            {
                this.Begin = begin;
                this.End = begin;
            }

            internal Range(int begin, int end)
            {
                this.Begin = begin;
                this.End = end;
            }

            public override string ToString()
            {
                return this.Begin == this.End ? this.Begin.ToString() : $"{this.Begin}-{this.End}";
            }
        }

        internal static string MinimizeRevisions(string revisions)
        {
            List<Range> ranges = GetRanges(revisions, @"(?<begin>\d+)(-(?<end>\d+))?");
            ranges = Minimize(ranges).ToList();
            string ret = ToString(ranges);
            return ret;
        }

        internal static string MinimizeMergeInfo(string mergeinfo)
        {
            var ss = Regex.Replace(mergeinfo, @"(.*):([^\r\n]+)(.*)", m =>
            {
                string ret = m.Value;
                var g = m.Groups[2];
                var s1 = g.Value;
                var s2 = MinimizeRevisions(g.Value);
                if (s1 != s2)
                    ret = $"{m.Groups[1]}:{s2}{m.Groups[3]}";
                return ret;
            });

            return ss;
        }

        static List<Range> GetRanges(string text, string pattern)
        {
            List<Range> ranges = new List<Range>();

            var mm = Regex.Matches(text, pattern);
            foreach (Match m in mm)
            {
                var begin = m.Groups["begin"].Value;
                var end = m.Groups["end"].Value;
                if (end == string.Empty)
                {
                    ranges.Add(new Range(int.Parse(begin)));
                }
                else
                {

                    ranges.Add(new Range(int.Parse(begin), int.Parse(end)));
                }
            }

            ranges.Sort((a, b) => a.Begin < b.Begin ? -1 : a.Begin > b.Begin ? 1 : 0);

            return ranges;
        }

        static IEnumerable<Range> Minimize(List<Range> ranges)
        {
            Range prev = ranges.FirstOrDefault();

            foreach (Range range in ranges.Skip(1))
            {
                if (range.Begin > prev.End + 1)
                {
                    var ret = prev;
                    prev = range;
                    yield return ret;
                }
                else
                {
                    Debug.Assert(range.Begin == prev.End + 1);
                    prev = new Range(prev.Begin, range.End);
                }
            }

            if (prev != null)
                yield return prev;
        }

        static string ToString(List<Range> ranges)
        {
            bool b = false;
            StringBuilder sb = new StringBuilder();
            foreach (var r in ranges)
            {
                if (b)
                    sb.Append(",");
                else
                    b = true;

                sb.Append(r.ToString());
            }

            string ret = sb.ToString();
            return ret;
        }
    }
}
