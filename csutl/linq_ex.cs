using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Linq
{
    static class Enumerable_ex
    {
        public static int IndexOf<T>(this IEnumerable<T> ee, Func<T, bool> predicate)
        {
            int ix = 0;
            var n = ee.FirstOrDefault(a => { ix++; return predicate(a); });
            return n == null ? -1 : ix - 1;
        }

        public static int IndexOf<T>(this IEnumerable<T> ee, T t) where T : class
        {
            int ix = ee.IndexOf<T>(a => a.Equals(t));
            return ix;
        }

        public static void Foreach<T>(this IEnumerable<T> ee, Action<T> action)
        {
            foreach (var a in ee)
                action(a);
        }


        /// <summary>
        /// return default if items Count != 1
        /// </summary>
        public static T SingleOr<T>(this IEnumerable<T> ee, T or = default(T))
        {
            T ret = or;
            int n = 0;
            foreach (var item in ee)
            {
                if (++n == 1)
                {
                    ret = item;
                }
                else
                {
                    ret = or;
                    break;
                }
            }
            return ret;
        }
    }
}

namespace linq_ex
{
    static class List_ex
    {
        public static void RemoveLast(this IList list)
        {
            list.RemoveAt(list.Count - 1);
        }

        public static void RemoveRest(this IList list, int maxCount)
        {
            while (list.Count > maxCount)
                list.RemoveAt(list.Count - 1);
        }

        public static void RemoveIf<T>(this IList<T> list, Func<T, bool> condition)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (condition(list[i]))
                    list.RemoveAt(i);
            }
        }

        public static void Swap(this IList list, int x, int y)
        {
            var tmp = list[x];
            list[x] = list[y];
            list[y] = tmp;
        }

        public static void Insert<T>(this List<T> list, int ix, IEnumerable<T> items)
        {
            int i = 0;
            foreach (var item in items)
                list.Insert(ix + i++, item);
        }

        public static T[] RemoveAt<T>(this List<T> list, int ix, int count)
        {
            var ar = new T[count];
            for (int i = 0; i < count; i++)
            {
                ar[i] = list[ix];
                list.RemoveAt(ix);
            }
            return ar;
        }

        public static void AddUnique<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void AddUnique<T>(this List<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
                list.AddUnique(item);
        }
    }

    static class Utl_ex
    {
        public static bool Empty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static char LastChar(this string s)
        {
            return s.Empty() ? (char)0 : s[s.Length - 1];
        }
        public static string WithEol2(this string s)
        {
            var c = s.LastChar();
            if (c != '\n' && c != '\r')
                s = s + "\r\n";
            return s;
        }

        public static string Replace(this string s, int ix, int len, string replace)
        {
            return $"{s.Substring(0, ix)}{replace}{s.Substring(ix + len)}";
        }

        public static int BeginOfLine(this string s, int ix) // return position of line
        {
            while (ix > 0 && s[ix - 1] != '\n' && s[ix - 1] != '\r')
                --ix;
            return ix;
        }

        public static int BeginOfSpaces(this string s, int ix) // return position of first space
        {
            while (ix > 0 && (s[ix - 1] == ' ' || s[ix - 1] == '\t'))
                --ix;
            return ix;
        }

        public static T GetOrAdd<Key, T>(this Dictionary<Key, T> dict, Key key, Func<T> get)
        {
            if (!dict.TryGetValue(key, out T t))
            {
                t = get();
                dict[key] = t;
            }
            return t;
        }
    }

    public class NodeT<T> where T : NodeT<T>
    {
        protected List<T> list;
        static List<T> empty = new List<T>();

        public T Parent { get; private set; }
        public IReadOnlyList<T> Nodes => this.list != null ? this.list : empty;
        public int Count => this.list != null ? this.list.Count : 0;
        public T this[int ix] => this.list[ix];
        public T Insert(int ix, T node)
        {
            if (node.Parent != null)
                throw new ArgumentException("NodeT<>.Insert: node.Parent != null", "node");
            if (this.list == null)
                this.list = new List<T>();
            this.list.Insert(ix, node);
            node.Parent = (T)this;
            return node;
        }

        public void RemoveAt(int ix)
        {
            var node = this.list[ix];
            node.Parent = null;
            this.list.RemoveAt(ix);
            if (this.list.Count == 0)
                this.list = null;
        }

        public void SetIndex(int ix)
        {
            if (this.Parent != null)
            {
                int prevIx = this.Parent.Nodes.IndexOf(this);
                if (prevIx != ix)
                {
                    this.Parent.list.RemoveAt(prevIx);
                    this.Parent.list.Insert(ix, (T)this);
                }
            }
        }
    }

    static class NodeT_ex
    {
        public static T Root<T>(this NodeT<T> node) where T : NodeT<T>
        {
            while (node.Parent != null)
                node = node.Parent;
            return (T)node;
        }

        public static void SetParent<T>(this NodeT<T> node, T parent, int ix = 0) where T : NodeT<T>
        {
            if (node.Parent != null)
                node.Parent.Remove((T)node);
            if (parent != null)
                parent.Insert(ix, (T)node);
        }

        public static void Remove<T>(this NodeT<T> prn, T node) where T : NodeT<T>
        {
            int ix = prn.Nodes.IndexOf(node);
            prn.RemoveAt(ix);
        }
        public static T Add<T>(this NodeT<T> prn, T node) where T : NodeT<T>
        {
            return prn.Insert(prn.Count, node);
        }

        public static int Index<T>(this NodeT<T> node) where T : NodeT<T>
        {
            return node.Parent != null ? node.Parent.Nodes.IndexOf(node) : -1;
        }

        public static T NextSibling<T>(this NodeT<T> node) where T : NodeT<T>
        {
            int ix = node.Index() + 1;
            return ix < node.Parent.Count ? node.Parent[ix] : null;
        }

        public static T PrevSibling<T>(this NodeT<T> node) where T : NodeT<T>
        {
            int ix = node.Index() - 1;
            return ix != -1 ? node.Parent[ix] : null;
        }

        public static T Clone<T>(this T node, Func<T, T> new_T) where T : NodeT<T>
        {
            T node2 = new_T(node);
            foreach (var n in node.Nodes)
                node2.Add(n.Clone(new_T));
            return node2;
        }

        public static IEnumerable<T> EnumParents<T>(this NodeT<T> node, bool me = false) where T : NodeT<T>
        {
            NodeT<T> n = me ? node : node.Parent;
            while (n != null)
            {
                yield return (T)n;
                n = n.Parent;
            }
        }

        public static IEnumerable<T> EnumChildren<T>(this IEnumerable<T> Nodes, bool recursive) where T : NodeT<T>
        {
            foreach (T n in Nodes)
            {
                yield return n;

                if (recursive)
                {
                    foreach (T nn in n.EnumChildren<T>(true))
                        yield return nn;
                }
            }
        }

        public static IEnumerable<T> EnumChildren<T>(this NodeT<T> node, bool recursive) where T : NodeT<T>
        {
            return node == null ? Enumerable.Empty<T>() : node.Nodes.EnumChildren<T>(recursive);
        }

        public static IEnumerable<T> EnumNodes<T>(this NodeT<T> node, bool me, bool childs, bool recursive) where T : NodeT<T>
        {
            Debug.Assert(!recursive || childs);

            if (me)
                yield return (T)node;

            if (childs)
            {
                foreach (T n in node.EnumChildren(recursive))
                    yield return n;
            }
        }
    }

    /// <summary>
    /// V - is item of View
    /// M - is item of Model
    /// </summary>
    static class TreeList_ex
    {
        static void DeleteNotExisting<V, M>(IList list_V, IEnumerable<M> list_M, Func<V, M> get_M_of_V) where M : class
        {
            var dictM = list_M.ToDictionary(m => m);
            for (int i = list_V.Count - 1; i != -1; --i)
            {
                V v = (V)list_V[i];
                var m = get_M_of_V(v);
                if (!dictM.ContainsKey(m))
                    list_V.RemoveAt(i);
            }
        }

        static void CreateNew<V, M>(IList list_V, IEnumerable<M> list_M, Func<M, V> new_V, Func<V, M> get_M_of_V) where M : class
        {
            var dict = list_V.Cast<V>().ToDictionary(get_M_of_V);
            int ix = 0;
            foreach (var m in list_M)
            {
                if (!dict.TryGetValue(m, out V v))
                {
                    v = new_V(m);
                    list_V.Insert(ix, v);
                }
                ix++;
            }
        }

        class Info
        {
            internal int cur_pos;
            internal int trg_pos;
            internal int offset => this.trg_pos - this.cur_pos;
        }

        static Dictionary<T, Info> CreateInfos<T>(IList<T> cur, IEnumerable<T> trg) where T : class
        {
            var dict = new Dictionary<T, Info>();
            int ix = 0;
            foreach (T b in trg)
            {
                T a = cur[ix];

                if (!dict.TryGetValue(a, out Info info))
                    info = dict[a] = new Info();
                info.cur_pos = ix;

                if (a != b && !dict.TryGetValue(b, out info))
                    info = dict[b] = new Info();
                info.trg_pos = ix;

                ix++;
            }
            Debug.Assert(ix == cur.Count);
            return dict;
        }

        class Group
        {
            internal Info info;
            internal int length;
        }

        static List<Group> CreateGroups<T>(IList<T> cur, Dictionary<T, Info> dict)
        {
            var gg = new List<Group>();
            if (cur.Count != 0)
            {
                for (int i = 0; i < cur.Count; i++)
                {
                    var g = i > 0 ? gg[gg.Count - 1] : null; // last group
                    var info = dict[cur[i]];

                    if (g != null && g.info.offset == info.offset)
                    {
                        g.length++;
                    }
                    else
                    {
                        g = new Group();
                        gg.Add(g);
                        g.info = info;
                        g.length = 1;
                    }
                }
            }
            return gg;
        }

        static void SortGroups(List<Group> gg)
        {
            gg.Sort((a, b) =>
            {
                if (a.length != b.length) // small length
                    return a.length < b.length ? -1 : 1;

                if (a.info.offset != b.info.offset) // big offset
                    return Math.Abs(a.info.offset) > Math.Abs(b.info.offset) ? -1 : 1;

                if (a.info.cur_pos != b.info.cur_pos) // small pos
                    return a.info.cur_pos < b.info.cur_pos ? -1 : 1;

                Debug.Assert(a == b);
                return 0;
            });
        }

        static void Merge(List<Group> gg)
        {
            for (int i = gg.Count - 2; i >= 0; i--)
            {
                var g1 = gg[i];
                var g2 = gg[i + 1];
                Debug.Assert(g1.info.cur_pos + g1.length == g2.info.cur_pos);
                if (g1.info.offset == g2.info.offset)
                {
                    g1.length += g2.length;
                    gg.RemoveAt(i + 1);
                }
            }
        }

        static void Move(Group g, List<Group> gg, Action<Group, int/*old pos*/> moved)
        {
            int old_pos = g.info.cur_pos;
            if (g.info.offset < 0)
            {
                int ix = gg.IndexOf(g);
                int i = ix - 1;
                while (g.info.offset != 0)
                {
                    Group g2 = gg[i];
                    g.info.cur_pos -= g2.length;
                    g2.info.cur_pos += g.length;
                    gg.Swap(i, i + 1);
                    --i;
                }
            }
            else if (g.info.offset > 0)
            {
                int ix = gg.IndexOf(g);
                int i = ix + 1;
                while (g.info.offset != 0)
                {
                    Group g2 = gg[i];
                    g.info.cur_pos += g2.length;
                    g2.info.cur_pos -= g.length;
                    gg.Swap(i, i - 1);
                    ++i;
                }
            }
            Debug.Assert(g.info.offset == 0);
            moved(g, old_pos);
        }

        static void Arrange<T>(IList<T> cur, IEnumerable<T> trg, Action<Group, int/*old pos*/> moved) where T : class
        {
            var dict = CreateInfos(cur, trg);
            List<Group> gg = CreateGroups(cur, dict);
            List<Group> ggSorted = gg.ToList();
            SortGroups(ggSorted);
            foreach (var g in ggSorted)
            {
                if (g.info.offset != 0)
                {
                    Move(g, gg, moved);
                }
            }
        }

        public static void update_list_from_model<V, M>(
            this IList list_V,     // nodes of view
            IEnumerable<M> list_M, // nodes of model
            Func<M, V> new_V,      // new V{ Tag = m }, without update
            Func<V, M> get_M_of_V, // (M)v.Tag
            Action<V, M> update_V  // update V from M
            ) where M : class
        {
            DeleteNotExisting(list_V, list_M, get_M_of_V);
            CreateNew(list_V, list_M, new_V, get_M_of_V);
            var list_VM = list_V.Cast<V>().Select(v =>
            {
                var m = get_M_of_V(v);
                update_V(v, m);
                return m;
            }
            ).ToList();
            Debug.Assert(list_VM.Count == list_M.Count());
            var tmp = new List<V>();
            Arrange(list_VM, list_M, (g, old_pos) =>
            {
                tmp.Clear();
                for (int k = 0; k < g.length; k++)
                {
                    tmp.Add((V)list_V[old_pos]);
                    list_V.RemoveAt(old_pos);
                }
                int i = 0;
                foreach (V v in tmp)
                    list_V.Insert(g.info.cur_pos + i++, v);
            });
        }

        public static void update_tree_from_model<V, M>(
            this IList list_V,                    // nodes of view
            IEnumerable<M> list_M,                // nodes of model
            Func<M, IEnumerable<M>> children_of_M,
            Func<V, IList> children_of_V,
            Func<M, V> new_V,                     // new V{ Tag = m }, without update
            Func<V, M> get_M_of_V,                // (M)v.Tag
            Action<V, M> update_V                 // update V from M
            ) where M : class
        {
            list_V.update_list_from_model(list_M, new_V, get_M_of_V, update_V);
            foreach (V v in list_V)
            {
                M m = get_M_of_V(v);
                children_of_V(v).update_tree_from_model<V, M>(
                    children_of_M(m),
                    children_of_M,
                    children_of_V,
                    new_V,
                    get_M_of_V,
                    update_V);
            }
        }

        public static void update_tree_from_model<V, M>(
            this IList list_V,            // nodes of view
            M tree,                       // NodeT<M>
            Func<V, IList> children_of_V,
            Func<M, V> new_V,             // new V{ Tag = m }
            Func<V, M> get_M_of_V,        // (M)v.Tag
            Action<V, M> update_V         // update V from M
            ) where M : NodeT<M>
        {
            list_V.update_tree_from_model<V, M>(
                tree.Nodes,
                a => a.Nodes,
                children_of_V,
                new_V,
                get_M_of_V,
                update_V);
        }
#if false // example
        treeView1.Nodes.update_tree_from_model2(
            root
            , a => (a as TreeNode).Nodes
            , a => new TreeNode(a.DisplayText) { Tag = a }
            , a => (Node)(a as TreeNode).Tag
            , (v, n) => { v.Text = n.DisplayText; }
            );
#endif
    }
}
