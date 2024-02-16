using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace csutl.ini.impl;

[DebuggerDisplay("{ToString()}")]
class Section : ini.Section
{
    internal IniFile ini { get; set; }
    string _name = string.Empty;
    string _comment = string.Empty;
    LinkedList<ItemImpl> items = new LinkedList<ItemImpl>();
    Dictionary<string, ItemImpl> map = new Dictionary<string, ItemImpl>();
    IModifiable Modifiable => this.ini as FileImpl;

    public bool IsName => !string.IsNullOrEmpty(this._name);
    public bool IsComment => !string.IsNullOrEmpty(this._comment);

    public string Name
    {
        get => this._name;
        private set => this.Modifiable.Modify(ref this._name, value);
    }

    public string Comment
    {
        get => this._comment;
        set => this.Modifiable.Modify(ref this._comment, value);
    }

    internal Section(string name, IniFile ini)
    {
        this.ini = ini;
        this._name = name;
    }

    public IEnumerable<Item> Items => this.items;

    public IEnumerable<Item> KeyValues => this.Items.Where(a => a.HasKey);

    public Item FindItem(string key)
    {
        ItemImpl item = null;
        if (!string.IsNullOrEmpty(key))
            this.map.TryGetValue(key, out item);
        return item;
    }

    public string Value(string key, object _defaultValue)
    {
        string ret = string.Empty;
        var item = this.FindItem(key);

        if (item != null)
            ret = item.Value;
        else if (_defaultValue != null)
            ret = _defaultValue.ToString();

        return ret;
    }

    public bool Bool(string key, bool _defaultValue)
    {
        bool ret = _defaultValue;
        var item = this.FindItem(key);

        if (item != null)
            ret = item.Bool;

        return ret;
    }

    public int Int(string key, int _defaultValue)
    {
        int ret = _defaultValue;
        var item = this.FindItem(key);

        if (item != null)
            ret = item.Int;

        return ret;
    }

    public float Float(string key, float _defaultValue)
    {
        float ret = _defaultValue;
        var item = this.FindItem(key);

        if (item != null)
            ret = item.Int;

        return ret;
    }

    public double Double(string key, double _defaultValue)
    {
        double ret = _defaultValue;
        var item = this.FindItem(key);

        if (item != null)
            ret = item.Int;

        return ret;
    }

    public Item this[string key, object _defaultValue, string _defaultComment] => this.Get(key, _defaultValue, _defaultComment);

    public Item Get(string key, object _defaultValue, string _defaultComment)
    {
        var item = this.FindItem(key);

        if (item == null)
            item = this.AddItem(key, _defaultValue, _defaultComment);

        return item;
    }

    public void Set(string key, object value, string comment)
    {
        Item item = this.Get(key, null, null);
        item.ValueX = value;
        item.Comment = comment;
    }

    public void Set(string key, string value, string comment)
    {
        Item item = this.Get(key, null, null);
        item.Value = value;
        item.Comment = comment;
    }

    internal Item AddItem(string key, object _defaultValue, string _defaultComment)
    {
        var item = new ItemImpl(this, key, _defaultValue, _defaultComment);
        this.items.AddLast(item);
        if (item.HasKey)
            this.map[key] = item;
        if (!this.Modifiable.Initializing)
            this.Modifiable.Modified = true;
        return item;
    }


    public void Clear()
    {
        this.items.Clear();
        this.map.Clear();
    }

    public IEnumerable<string> GetText(bool withoutComments)
    {
        int commentAlign = 0;
        if (withoutComments)
            commentAlign = -1;
        else if (this.ini.Format.CommentAlignMaxPos > 0)
            commentAlign = (this.ini.Format as FormatImpl).CalculateCommentAllign(this);

        var s = this.GeSectionNameText(commentAlign);
        if (this.Name != string.Empty)
            yield return s;

        bool keyFound = false;
        bool sub = false;
        foreach (var item in this.items)
        {
            if (item.HasKey)
            {
                keyFound = true;
                sub = false;
            }
            else if (keyFound)
            {
                sub = true;
            }
            int indent = sub ? this.ini.Format.SubItemIndent : this.ini.Format.ItemIndent;
            yield return item.GetText(indent, commentAlign);
        }
    }

    internal void SetSubItemsCount(ItemImpl item, int count)
    {
        bool xx = item.Next != null && !item.Next.HasKey && count > 0;
        if (xx)
            this.Modifiable.Modified = true;

        while (xx)
        {
            count--;
            item = item.Next;
            xx = item.Next != null && !item.Next.HasKey && count > 0;
        }

        if (count > 0)
        {
            this.items.AddAfter(item, newItem(count));
            IEnumerable<ItemImpl> newItem(int n)
            {
                while (n-- != 0)
                    yield return new ItemImpl(this);
            }
            this.Modifiable.Modified = true;
        }
        else
        {
            bool x = item.Next != null && !item.Next.HasKey;
            if (x)
                this.Modifiable.Modified = true;

            while (x)
            {
                this.items.Remove(item.Next);
                x = item.Next != null && !item.Next.HasKey;
            }
        }
    }

    internal void TrimEnd()
    {
        while (this.items.Last != null && this.items.Last.IsEmpty)
            this.items.Remove(this.items.Last);
    }

    public override string ToString()
    {
        return this.GeSectionNameText(0);
    }

    private string GeSectionNameText(int commentAlign)
    {
        return SB.BuildString(sb =>
        {
            FormatImpl.FormatSection(sb, this, commentAlign);
            return sb.ToString();
        });
    }
}

