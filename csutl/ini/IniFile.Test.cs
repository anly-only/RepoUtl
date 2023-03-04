using csutl;
using csutl.ini;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class IniFileTest
    {
        string n = "\n";


        [TestMethod]
        public void TestEmptyLines()
        {
            var i = Factory.Get();
            i.Format.BlankLineAfterSection = false;

            i.LoadIniBody($"");
            Assert.IsTrue(i.Sections.Count() == 1);
            Assert.IsTrue(i.Sections.First().Name == "");
            Assert.IsTrue(i.Sections.First().Items.Count() == 0);
            Check(i, sec: "", com1: "", key: "", val: "", com2: "");
            Assert.IsFalse(i.Modified);

            i.LoadIniBody($"{n}");
            Assert.IsTrue(i.Sections.Count() == 1);
            Assert.IsTrue(i.Sections.First().Name == "");
            Assert.IsTrue(i.Sections.First().Items.Count() == 1);
            var x = i.Sections.First().Items.First();
            Assert.IsTrue(x.Value == "" && x.Key == "" && x.Comment == "");
            Assert.IsTrue(!x.IsValue && !x.IsKey && !x.IsComment);
            Assert.IsFalse(i.Modified);

            i.LoadIniBody($"{n}{n}{n}");
            Assert.IsTrue(i.Sections.Count() == 1);
            Assert.IsTrue(i.Sections.First().Name == "");
            Assert.IsTrue(i.Sections.First().Items.Count() == 3);
            foreach (var xx in i.Sections.First().Items)
            {
                Assert.IsTrue(xx.Value == "" && xx.Key == "" && xx.Comment == "");
                Assert.IsTrue(!xx.IsValue && !xx.IsKey && !xx.IsComment);
            }
            Assert.IsTrue(IsEQ(i.GetText(), "", "", ""));
            Assert.IsFalse(i.Modified);
        }


        [TestMethod]
        public void TestBlankLineAfterSection()
        {
            var i = Factory.Get();
            i.Format.ItemIndent = 4;
            i.Format.CommentAlignMaxPos = 0;

            i.LoadIniBody(T(
                "[aaa]    //    oooo  ",
                "bbb = 1  //   ccc  ",
                "[qqq]",
                "yyy = 3  //   zzz  "
                ));

            Assert.IsTrue(IsEQ(i.GetText(),
                "[aaa] // oooo",
                "    bbb = 1 // ccc",
                "",    // blank line
                "[qqq]",
                "    yyy = 3 // zzz",
                ""    // blank line
                ));

            Check(i, sec: "aaa", com1: "oooo", key: "bbb", val: "1", com2: "ccc");
            Check(i, sec: "qqq", com1: "", key: "yyy", val: "3", com2: "zzz");
            Assert.IsFalse(i.Modified);
        }

        [TestMethod]
        public void TestEmptyLineAfterSection2()
        {
            var i = Factory.Get();
            i.Format.BlankLineAfterSection = true;
            i.Format.ItemIndent = 4;
            i.Format.CommentAlignMaxPos = 0;

            // by loading the last empty lines are ignored
            i.LoadIniBody(T(
                " [aaa]    //    oooo  ",
                " bbb = 1  //   ccc  ",
                " ",
                " ",
                "",
                " [qqq]",
                " yyy     = 3  //   zzz  ",
                "",
                "",
                ""
                ));

            // by saving one empty line is added at the end
            Assert.IsTrue(IsEQ(i.GetText(),
                "[aaa] // oooo",
                "    bbb = 1 // ccc",
                "",
                "[qqq]",
                "    yyy = 3 // zzz",
                ""
                ));

            Check(i, sec: "aaa", com1: "oooo", key: "bbb", val: "1", com2: "ccc");
            Check(i, sec: "qqq", com1: "", key: "yyy", val: "3", com2: "zzz");
            Assert.IsFalse(i.Modified);
        }

        [TestMethod]
        public void TestQuotes()
        {
            var i = Factory.Get();
            i.Format.ItemIndent = 4;
            i.Format.CommentAlignMaxPos = 0;

            i.LoadIniBody(T(
                $" [a'[x//x]'aa]    //    oooo  ",
                $" bb'=''='b = 1  //   ccc  ",
                ""
                ));

            i.Format.Quotes = Quotes.Full;
            Assert.IsTrue(IsEQ(i.GetText(),
                $"['a[x//x]aa'] // oooo",
                $"    'bb=''=b' = 1 // ccc",
                ""
                ));

            Check(i, sec: "a[x//x]aa", com1: "oooo", key: "bb='=b", val: "1", com2: "ccc");
            Assert.IsFalse(i.Modified);
        }

        [TestMethod]
        public void TestEols()
        {
            var i = Factory.Get();
            i.Format.ItemIndent = 4;
            i.Format.CommentAlignMaxPos = 0;

            i.LoadIniBody(T(
                $" ['a\naa']    //    'oo\noo'  ",
                $" 'bb\nb' = '11\n11'  //   'cc\ncc'  ",
                ""
                ));

            i.Format.Quotes = Quotes.Full;
            Assert.IsTrue(IsEQ(i.GetText(),
                $"['a\naa'] // 'oo\noo'",
                $"    'bb\nb' = '11\n11' // 'cc\ncc'",
                ""
                ));

            Check(i, sec: "a\naa", com1: "oo\noo", key: "bb\nb", val: "11\n11", com2: "cc\ncc");
            Assert.IsFalse(i.Modified);
        }

        [TestMethod]
        public void TestIndent()
        {
            var i = Factory.Get();
            i.Format.ItemIndent = 3;
            i.Format.SubItemIndent = 6;

            i.LoadIniBody(T(
                " [aaa]",
                " 1",
                " 1",
                " key = 2",
                " 2",
                " 2",
                " key = 3",
                " 3",
                " 3"
                ));

            var x = i.GetText().ToArray();
            Assert.IsTrue(IsEQ(x,
                "[aaa]",
                "   1",
                "   1",
                "   key = 2",
                "      2",
                "      2",
                "   key = 3",
                "      3",
                "      3",
                ""
                ));
            Assert.IsFalse(i.Modified);
        }

        [TestMethod]
        public void TestIndent2()
        {
            var i = Factory.Get();
            i.Format.ItemIndent = 3;
            i.Format.SubItemIndent = 6;

            i.LoadIniBody(T(
                " [aaa]",
                " key = 2",
                " 2",
                " 2",
                " key = 3",
                " 3",
                " 3"
                ));

            var x = i.GetText().ToArray();
            Assert.IsTrue(IsEQ(x,
                "[aaa]",
                "   key = 2",
                "      2",
                "      2",
                "   key = 3",
                "      3",
                "      3",
                ""
                ));
            Assert.IsFalse(i.Modified);
        }

        [TestMethod]
        public void TestGroups()
        {
            var i = Factory.Get();
            i.Format.ItemIndent = 3;
            i.Format.SubItemIndent = 6;

            i.LoadIniBody(T(
                " [aaa] ",
                "    bbb = "
                ));

            var x = i["aaa"]["bbb"];
            Assert.IsTrue(!x.SubItems().Any());

            i.LoadIniBody(T(
                " [aaa] ",
                "    bbb = ",
                "       qqq",
                "       www",
                "    ccc = rrr"
                ));

            x = i["aaa"]["bbb"];
            Assert.IsTrue(x.SubItems().Count() == 2);
            Assert.IsTrue(IsEQ(x.SubItems().Select(a => a.Value), E("qqq", "www")));
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsFalse(i.Modified);

            x.SetSubItemsCount(4);
            Assert.IsTrue(x.SubItems().Count() == 4);
            Assert.IsTrue(IsEQ(x.SubItems().Select(a => a.Value), E("qqq", "www", "", "")));
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SubItems().Skip(2).First().Value = "eee";
            x.SubItems().Skip(3).First().Value = "rrr";
            Assert.IsTrue(IsEQ(x.SubItems().Select(a => a.Value), E("qqq", "www", "eee", "rrr")));
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SetSubItemsCount(2);
            Assert.IsTrue(IsEQ(x.SubItems().Select(a => a.Value), E("qqq", "www")));
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SetSubValues(Enumerable.Empty<string>());
            Assert.IsTrue(x.SubItems().Count() == 0);
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SetSubValues(E("qqq"));
            Assert.IsTrue(x.SubItems().Count() == 1);
            Assert.IsTrue(IsEQ(x.SubItems().Select(a => a.Value), E("qqq")));
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SetSubValues(E("qqq", "www", "eee"));
            Assert.IsTrue(x.SubItems().Count() == 3);
            Assert.IsTrue(IsEQ(x.SubItems().Select(a => a.Value), E("qqq", "www", "eee")));
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SetSubValues(E("qqq"));
            Assert.IsTrue(x.SubItems().Count() == 1);
            Assert.IsTrue(IsEQ(x.SubItems().Select(a => a.Value), E("qqq")));
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SetGroupValues(Enumerable.Empty<string>());
            Assert.IsTrue(!x.GroupValues().Any());
            Assert.IsTrue(x.SubItems().Count() == 0);
            Assert.IsTrue(x.Value == "");
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SetGroupValues(E("qqq"));
            Assert.IsTrue(IsEQ(x.GroupValues(), E("qqq")));
            Assert.IsTrue(x.SubItems().Count() == 0);
            Assert.IsTrue(x.Value == "qqq");
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SetGroupValues(E("qqq\nwww"));
            Assert.IsTrue(IsEQ(x.GroupValues(), E("qqq\nwww")));
            Assert.IsTrue(x.SubItems().Count() == 1);
            Assert.IsTrue(x.Value == "");
            Assert.IsTrue(IsEQ(x.SubItems().Select(a => a.Value), E("qqq\nwww")));
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SetGroupValues(E("qqq", "www"));
            Assert.IsTrue(IsEQ(x.GroupValues(), E("qqq", "www")));
            Assert.IsTrue(x.SubItems().Count() == 2);
            Assert.IsTrue(x.Value == "");
            Assert.IsTrue(IsEQ(x.SubItems().Select(a => a.Value), E("qqq", "www")));
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SetGroupValue("qqq\nwww\neee");
            Assert.IsTrue(IsEQ(x.GroupValues(), E("qqq", "www", "eee")));
            Assert.IsTrue(x.SubItems().Count() == 3);
            Assert.IsTrue(x.Value == "");
            Assert.IsTrue(IsEQ(x.SubItems().Select(a => a.Value), E("qqq", "www", "eee")));
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SetGroupValue("\n\n");
            Assert.IsTrue(IsEQ(x.GroupValues(), E("", "", "")));
            Assert.IsTrue(x.SubItems().Count() == 3);
            Assert.IsTrue(x.Value == "");
            Assert.IsTrue(IsEQ(x.SubItems().Select(a => a.Value), E("", "", "")));
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            x.SetGroupValue("qqq");
            Assert.IsTrue(IsEQ(x.GroupValues(), E("qqq")));
            Assert.IsTrue(x.SubItems().Count() == 0);
            Assert.IsTrue(x.Value == "qqq");
            Assert.IsTrue(i["aaa"].FindItem("ccc") != null);
            Assert.IsTrue(i.Modified);
            i.Modified = false;
        }

        [TestMethod]
        public void TestFormatCommentAsItem()
        {
            var i = Factory.Get();
            i.Format.ItemIndent = 3;
            i.Format.SubItemIndent = 6;

            i.LoadIniBody(T(
                "//1"
                ));

            var x = i.GetText().ToArray();
            string xx = string.Join("\n", x);
            Assert.IsTrue(IsEQ(x,
                "   // 1",
                ""
                ));
            Assert.IsFalse(i.Modified);

            i.LoadIniBody(T(
                " [aaa]//1",
                "//1"
                ));

            x = i.GetText().ToArray();
            xx = string.Join("\n", x);
            Assert.IsTrue(IsEQ(x,
                "[aaa] // 1",
                "   // 1",
                ""
                ));
            Assert.IsFalse(i.Modified);

            i.LoadIniBody(T(
                "//1",
                " [aaa]//1",
                "//1"
                ));

            x = i.GetText().ToArray();
            xx = string.Join("\n", x);
            Assert.IsTrue(IsEQ(x,
                "   // 1",
                "",
                "[aaa] // 1",
                "   // 1",
                ""
                ));
            Assert.IsFalse(i.Modified);

            i.LoadIniBody(T(
                "//1",
                "key=1//1",
                " [aaa]//1",
                "//1",
                "key=1//1"
                ));

            x = i.GetText().ToArray();
            xx = string.Join("\n", x);
            Assert.IsTrue(IsEQ(x,
                "   // 1",
                "   key = 1 // 1",
                "",
                "[aaa]      // 1",
                "   // 1",
                "   key = 1 // 1",
                ""
                ));
            Assert.IsFalse(i.Modified);
        }

        [TestMethod]
        public void TestAdd()
        {
            var i = Factory.Get();
            Assert.IsFalse(i.Modified);

            var s = i["aaa"];
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            var ss = s["bbb"];
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            ss.Int = 123;
            Assert.IsTrue(i.Modified);
            i.Modified = false;

            var x = i.FindSection("aaa").FindItem("bbb");
            Assert.IsTrue(x.Int == 123);
            Assert.IsFalse(i.Modified);
        }

        void Check(IniFile ini, string sec = "", string com1 = "", string key = "", string val = "", string com2 = "")
        {
            var s = ini.FindSection(sec);
            Assert.IsTrue(s != null);
            Assert.IsTrue(s.Comment == com1);

            Item i = null;
            if (!string.IsNullOrEmpty(key))
            {
                i = s.KeyValues.FirstOrDefault(a => a.Key == key);
                Assert.IsTrue(i != null);
            }
            else if (!string.IsNullOrEmpty(val) || !string.IsNullOrEmpty(com2))
            {
                i = s.Items.FirstOrDefault();
                Assert.IsTrue(i != null);
            }

            if (i != null)
            {
                Assert.IsTrue(i.Value == val);
                Assert.IsTrue(i.Comment == com2);
            }
        }

        bool IsEQ(IEnumerable<string> a, string first, params string[] b) => a.SequenceEqual(Enumerable.Repeat(first, 1).Concat(b));
        bool IsEQ(IEnumerable<string> a, IEnumerable<string> b) => a.SequenceEqual(b);

        IEnumerable<string> E(params string[] a) => a;

        string T(params string[] a) => string.Join('\n', a) + '\n';
    }
}
