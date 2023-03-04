using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace csutl
{
    public interface ILinkedListNode<T>
    {
        T Prev { get; }
        T Next { get; }
        void LinkedList_SetNode(bool next, T node);
    }

    public class LinkedListNode<T> : ILinkedListNode<T>
    {
        public T Prev { get; private set; }
        public T Next { get; private set; }

        void ILinkedListNode<T>.LinkedList_SetNode(bool next, T node)
        {
            if (next)
                Next = node;
            else
                Prev = node;
        }
    }

    public static class ILinkedListNodeEx
    {
        public static IEnumerable<T> GetNexts<T>(this T @this, bool includeThis = false)
            where T : ILinkedListNode<T>
        {
            var node = includeThis ? @this : @this.Next;
            while (node != null)
            {
                yield return node;
                node = node.Next;
            }
        }

        public static IEnumerable<T> GetPrevs<T>(this T @this, bool includeThis = false)
            where T : ILinkedListNode<T>
        {
            var node = includeThis ? @this : @this.Prev;
            while (node != null)
            {
                yield return (T)node;
                node = node.Prev;
            }
        }
    }

    public class LinkedList<T> : IEnumerable<T> where T : ILinkedListNode<T>
    {
        public T First { get; private set; }
        public T Last { get; private set; }

        public int Count { get; private set; }

        public void Clear()
        {
            First = default;
            Last = default;
            Count = 0;
        }

        public void AddFirst(T node) => AddBefore(First, node);
        public void AddLast(T node) => AddAfter(Last, node);

        public void AddBefore(T node, T add)
        {
            Debug.Assert(add != null);
            Debug.Assert(node == null || this.Contains(node));
            Debug.Assert(!this.Contains(add));

            if (node == null)
                node = First;

            T oldPrev = default;

            if (node != null)
            {
                oldPrev = node.Prev;
                disconnect(node.Prev, node);
            }

            connect(oldPrev, add);
            connect(add, node);

            if (add.Prev == null)
                First = add;

            if (add.Next == null)
                Last = add;

            Count++;
        }

        public void AddAfter(T node, T add)
        {
            Debug.Assert(add != null);
            Debug.Assert(node == null || this.Contains(node));
            Debug.Assert(!this.Contains(add));

            if (node == null)
                node = Last;

            T oldNext = default;

            if (node != null)
            {
                oldNext = node.Next;
                disconnect(node, node.Next);
            }

            connect(node, add);
            connect(add, oldNext);

            if (add.Prev == null)
                First = add;

            if (add.Next == null)
                Last = add;

            Count++;
        }

        public T AddAfter(T node, IEnumerable<T> add) // return last
        {
            if (node == null)
                node = Last;

            foreach (var item in add)
            {
                AddAfter(node, item);
                node = item;
            }

            return node;
        }

        public void Remove(T node)
        {
            Debug.Assert(node != null);
            Debug.Assert(this.Contains(node));

            T p = node.Prev;
            T n = node.Next;

            disconnect(p, node);
            disconnect(node, n);
            connect(p, n);

            if (object.ReferenceEquals(node, First))
                First = n;

            if (object.ReferenceEquals(node, Last))
                Last = p;

            Count--;
        }

        static void disconnect(T prev, T next)
        {
            Debug.Assert(prev == null || object.ReferenceEquals(prev.Next, next));
            Debug.Assert(next == null || object.ReferenceEquals(prev, next.Prev));

            if (prev != null)
                prev.LinkedList_SetNode(true, default);

            if (next != null)
                next.LinkedList_SetNode(false, default);
        }

        static void connect(T prev, T next)
        {
            Debug.Assert(prev == null || prev.Next == null);
            Debug.Assert(next == null || next.Prev == null);

            if (prev != null)
                prev.LinkedList_SetNode(true, next);

            if (next != null)
                next.LinkedList_SetNode(false, prev);
        }

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
        IEnumerable<T> _items => First == null ? Enumerable.Empty<T>() : First.GetNexts(true);
    }
}
