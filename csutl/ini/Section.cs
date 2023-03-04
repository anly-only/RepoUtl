using System.Collections.Generic;

namespace csutl.ini;

public interface Section
{
    bool IsName { get; }
    bool IsComment { get; }

    string Name { get; }
    string Comment { get; set; }

    IEnumerable<Item> Items { get; }
    IEnumerable<Item> KeyValues { get; }

    string Value(string key, object _defaultValue = null);
    bool Bool(string key, bool _defaultValue = false);
    int Int(string key, int _defaultValue = 0);
    float Float(string key, float _defaultValue = 0);
    double Double(string key, double _defaultValue = 0);

    Item FindItem(string key);

    Item Get(string key, object _defaultValue = null, string _defaultComment = "");
    Item this[string key, object _defaultValue = null, string _defaultComment = ""] { get; }

    void Set(string key, object value, string comment = "");
    void Set(string key, string value, string comment = "");
}
