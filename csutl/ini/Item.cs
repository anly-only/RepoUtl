using System.Collections.Generic;

namespace csutl.ini;

public interface Item
{
    bool IsKey { get; }
    bool IsValue { get; }
    bool IsComment { get; }

    string Key { get; }
    string Comment { get; set; }

    string Value { get; set; }
    object ValueX { set; }
    bool Bool { get; set; }
    int Int { get; set; }
    double Double { get; set; }
    float Float { get; set; }

    IEnumerable<Item> Group();
    IEnumerable<Item> SubItems();
    IEnumerable<string> GroupValues();

    void SetGroupValue(string value);
    void SetGroupValues(IEnumerable<string> value);
    void SetSubValues(IEnumerable<string> value);
    void SetSubItemsCount(int count);
}
