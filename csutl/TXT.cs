using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace csutl
{
    static class TXT
    {
        internal static bool SelectWord(string text, int pos, out int begin, out int end)
        {
            begin = end = pos;

            while (end < text.Length && char.IsLetterOrDigit(text[end]))
                ++end;

            while (begin != 0 && char.IsLetterOrDigit(text[begin - 1]))
                --begin;

            return begin != end;
        }

        internal static string TabsToSpaces(int tab_size)
        {
            string s = string.Empty;
            for (int i = 0; i < tab_size; i++)
                s += " ";
            return s;
        }
        internal static string RN_to_N(this string s)
        {
            return s.Replace("\r\n", "\n");
        }

        internal static string N_to_RN(this string s)
        {
            return s.Replace("\n", "\r\n");
        }

        internal static string to_RN(this string s)
        {
            return s.RN_to_N().N_to_RN();
        }

        internal static char LastChar(this string s)
        {
            char c = '\0';
            if (!string.IsNullOrEmpty(s))
            {
                c = s[s.Length - 1];
            }
            return c;
        }

        internal static int SpacesLength(string s, int pos)
        {
            char space = ' ';
            int j = pos + 1;
            for (; j != s.Length && s[j] == space; j++)
                ;
            return j - pos;
        }

        #region convert
        internal static int[] ToIntArray(string arrayAsString, char separator)
        {
            char[] c = { separator };
            return Array.ConvertAll(arrayAsString.Split(c, StringSplitOptions.RemoveEmptyEntries), int.Parse);
        }

        internal static string ToString(int[] array, char separator)
        {
            char[] c = { separator };
            return string.Join(",", Array.ConvertAll(array, s => s.ToString()));
        }
        #endregion convert

        #region search
        internal static int IndexOf(List<string> list, string s, bool ignoreCase)
        {
            int ix = -1;
            for (int i = 0; ix == -1 && i != list.Count; ++i)
            {
                if (string.Compare(list[i], s, ignoreCase) == 0)
                    ix = i;
            }
            return ix;
        }
        internal static int LastIndexOf(List<string> list, string s, bool ignoreCase)
        {
            int ix = -1;
            for (int i = list.Count - 1; ix == -1 && i != -1; --i)
            {
                if (string.Compare(list[i], s, ignoreCase) == 0)
                    ix = i;
            }
            return ix;
        }

        public static int IndexOfAny(this string text, params string[] items)
        {
            return text.IndexOfAny((IEnumerable<string>)items);
        }

        public static int IndexOfAny(this string text, IEnumerable<string> items)
        {
            for (int i = 0; i < text.Length; i++)
            {
                foreach (var item in items)
                {
                    if (text.IsEqual(i, item))
                        return i;
                }
            }
            return -1;
        }
        #endregion search

        public static string Replace(this string text, params (string, string)[] pairs)
        {
            IEnumerable<(string, string)> e = pairs;
            return text.Replace(e);
        }

        public static string Replace(this string text, IEnumerable<(string, string)> pairs)
        {
            DebugCheckOverlap(pairs.Select(a => a.Item1));

            string result = SB.BuildString(sb =>
            {
                int lastIndex = 0;
                int ix = 0;
                while (ix < text.Length)
                {
                    bool changed = false;
                    foreach (var item in pairs)
                    {
                        if (item.Item1.Length != 0 && IsEqual(text, ix, item.Item1))
                        {
                            sb.Append(text.Substring(lastIndex, ix - lastIndex));
                            sb.Append(item.Item2);
                            ix += item.Item1.Length;
                            lastIndex = ix;
                            changed = true;
                            break;
                        }
                    }
                    if (!changed)
                        ++ix;
                }

                string ret;
                if (sb.Length != 0)
                {
                    sb.Append(text.Substring(lastIndex, ix - lastIndex));
                    ret = sb.ToString();
                }
                else
                {
                    ret = text;
                }

                return ret;
            });

            return result;
        }

        public static bool IsEqual(this string text, int startIndex, string value)
        {
            if (startIndex > text.Length)
                throw new ArgumentException($"{nameof(startIndex)} is out of range", nameof(startIndex));

            if (text.Length - startIndex < value.Length)
                return false;
            else if (value.Length == 0)
                return true;

            bool yes = false;

            int ix2 = 0;
            while (true)
            {
                if (ix2 == value.Length)
                {
                    yes = true;
                    break;
                }

                if (startIndex == text.Length)
                    break;

                if (text[startIndex] != value[ix2])
                    break;

                ++startIndex;
                ++ix2;
            }

            return yes;
        }

        [Conditional("DEBUG")]
        private static void DebugCheckOverlap(IEnumerable<string> list)
        {
            var ar = list.ToArray();
            for (int i = 0; i < ar.Length - 1; i++)
            {
                string s1 = ar[i];
                for (int j = i + 1; j < ar.Length; j++)
                {
                    string s2 = ar[j];
                    if (s2.IndexOf(s1, StringComparison.InvariantCulture) != -1)
                    {
                        // how to solve the problem:
                        // - remove duplicates in list
                        // - sort list by comparing of item.Length (first - longest, last - shortest)
                        Debug.Assert(false);
                    }
                }
            }
        }
    }

    public static class SB
    {
        static object _lock = new object();
        static List<StringBuilder> builders = new List<StringBuilder>();
        static Stack<int> free = new Stack<int>();

        public static string BuildString(Func<StringBuilder, string> act)
        {
            StringBuilder sb;
            int ix;
            lock (_lock)
            {
                if (free.Count != 0)
                {
                    ix = free.Pop();
                    sb = builders[ix];
                    if (sb == null)
                        builders[ix] = sb = new StringBuilder();
                }
                else
                {
                    ix = builders.Count;
                    sb = new StringBuilder();
                    builders.Add(sb);
                }
            }

            string ret = act.Invoke(sb);
            sb.Clear();

            lock (_lock)
                free.Push(ix);

            return ret;
        }

        public static void TrimExcess() // free allocated memory if possible
        {
            lock (_lock)
            {
                bool clear = builders.Count == free.Count;

                if (!clear)
                {
                    foreach (var ix in free)
                        builders[ix] = null;

                    clear = builders.All(a => a == null);
                }

                if (clear) // remove all builders
                {
                    builders.Clear();
                    free.Clear();
                }
                else
                {
                    // really seems no difference: NULLs are elements or they are in capacity buffer.
                    // in any case, remove free builders in the tail.
                    while (free.Count != 0 && free.Peek() == builders.Count - 1) // is the last builder free ?
                    {
                        builders.RemoveAt(builders.Count - 1); // remove the last builder
                        free.Pop();
                    }
                }
            }
        }
    }
}
