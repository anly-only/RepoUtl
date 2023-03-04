using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace csutl.ini.impl;

[DebuggerDisplay("{ToString()}")]
class ItemImpl : LinkedListNode<ItemImpl>, Item
{
    private string _key = string.Empty;
    private string _value = string.Empty;
    private string _comment = string.Empty;

    internal Section Section { get; private set; }
    private IModifiable Modifiable => this.Section.ini as FileImpl;

    public bool IsKey => !string.IsNullOrEmpty(this._key);
    public bool IsValue => !string.IsNullOrEmpty(this._value);
    public bool IsComment => !string.IsNullOrEmpty(this._comment);
    public bool IsEmpty => !this.IsKey && !this.IsValue && !this.IsComment;

    public string Key { get => this._key; set => this.Modifiable.Modify(ref this._key, value); }
    public string Value { get => this._value; set => this.Modifiable.Modify(ref this._value, value); }
    public object ValueX { set => this.Modifiable.Modify(ref this._value, value == null ? string.Empty : value.ToString()); }
    public string Comment { get => this._comment; set => this.Modifiable.Modify(ref this._comment, value); }

    public bool Bool
    {
        get
        {
            bool.TryParse(this._value, out bool result);
            return result;
        }
        set => this.Value = value.ToString();
    }

    public int Int
    {
        get
        {
            int.TryParse(this._value, out int result);
            return result;
        }
        set => this.Value = value.ToString();
    }

    public double Double
    {
        get
        {
            double.TryParse(this._value, out double result);
            return result;
        }
        set => this.Value = value.ToString();
    }

    public float Float
    {
        get
        {
            float.TryParse(this._value, out float result);
            return result;
        }
        set => this.Value = value.ToString();
    }

    internal ItemImpl(Section sec) => this.Section = sec;

    internal ItemImpl(Section sec, string key, object value, string comment)
    {
        this.Section = sec;
        this._key = key;
        this._value = value == null ? string.Empty : value.ToString(); ;
        this._comment = comment;
    }


    public IEnumerable<Item> Group() // return this and nexts without Key
    {
        return Enumerable.Repeat(this, 1).Concat(this.SubItems());
    }

    public IEnumerable<Item> SubItems() // return nexts without Key
    {
        var e = this.Section.Items.SkipWhile(a => a != this);
        if (!e.Any())
            throw new Exception();
        e = e.Skip(1);
        return e.TakeWhile(a => !a.IsKey);
    }

    public IEnumerable<string> GroupValues()
    {
        var e = this.Group();
        if (e.Any() && e.First().IsKey && !e.First().IsValue)  // first can be empty. Like: key = // comment
            e = e.Skip(1);
        return e.Select(a => a.Value);
    }

    public void SetGroupValue(string value)
    {
        this.SetGroupValues(Regex.Split(value, "\r\n|\n"));
    }

    public void SetGroupValues(IEnumerable<string> value)
    {
        int newCount = value.Count();

        if (newCount == 0)
        {
            this.Value = string.Empty;
            this.SetSubItemsCount(0);
        }
        else if (newCount == 1 && !value.First().Contains('\n'))
        {
            this.Value = value.First();
            this.SetSubItemsCount(0);
        }
        else
        {
            this.Value = string.Empty;
            this.SetSubItemsCount(newCount);
            this.SetSubValues(value);
        }
    }

    public void SetSubValues(IEnumerable<string> value)
    {
        int newCount = value.Count();
        this.SetSubItemsCount(newCount);
        var en = value.GetEnumerator();
        var en2 = this.SubItems().GetEnumerator();

        for (int i = 0; i < newCount; i++)
        {
            en.MoveNext();
            en2.MoveNext();
            en2.Current.Value = en.Current;
        }
    }

    public void SetSubItemsCount(int count) => this.Section.SetSubItemsCount(this, count);

    public override string ToString()
    {
        return this.GetText(0, 0);
    }

    public string GetText(int indent, int commentAlign)
    {
        return SB.BuildString(sb =>
        {
            FormatImpl.FormatItem(sb, this, indent, commentAlign);
            return sb.ToString();
        });
    }
}

