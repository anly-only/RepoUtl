using csutl.ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace cmd_ini
{
    public class Cmd
    {
        public CmdIni Ini { get; }

        public string Name { get; set; }
        public string File { get; set; }
        public string Args { get; set; }
        public bool Enabled { get; set; } = true;
        public bool WaitForExit { get; set; }
        public bool IsSeparator { get; set; }

        public Cmd(CmdIni ini)
        {
            this.Ini = ini;
        }

        public Cmd Clone()
        {
            return new Cmd(this.Ini)
            {
                Name = this.Name,
                File = this.File,
                Args = this.Args,
                Enabled = this.Enabled,
                WaitForExit = this.WaitForExit,
                IsSeparator = this.IsSeparator
            };
        }
    }

    public class CmdIni
    {
        public class Key
        {
            public const string File = "File";
            public const string Args = "Args";
            public const string Visible = "Visible";
            public const string Enabled = "Enabled";
            public const string WaitForExit = "WaitForExit";
            public const string OR = "OR";
            public const string AND = "AND";
            public const string NOT = "NOT";
        }

        public class Condition
        {
            public const string FolderExists = "folder_exists";
            public const string FileExists = "file_exists";
        }

        const string macroPattern = "<((?:\\w|-)+)>";
        const string argumentPattern = "<(\\d+)>";
        Action<string> Report { get; }

        public ICmdMacros Macros { get; }
        public string FileName { get; private set; }


        public CmdIni(string file, ICmdMacros macros, Action<string> Report)
        {
            this.FileName = file;
            this.Macros = macros;
            this.Report = Report;
        }

        public IEnumerable<Cmd> GetCommands()
        {
            var ini = this.GetIni(this.FileName);

            foreach (var section in ini.Sections.Where(a => !string.IsNullOrEmpty(a.Name)))
            {
                var c = new Cmd(this);
                if (this.GetBoolExpr(section, Key.Visible, true))
                {
                    if (!section.Items.Any())
                    {
                        c.IsSeparator = true;
                    }
                    else
                    {
                        c.Name = section.Name;
                        c.File = section.Value(Key.File);
                        c.Args = section.Value(Key.Args);
                        c.Enabled = this.GetBoolExpr(section, Key.Enabled, true);
                        c.WaitForExit = this.GetBoolExpr(section, Key.WaitForExit, false);
                    }
                    yield return c;
                }
            }
        }

        IniFile GetIni(string iniPath)
        {
            var ini = IniFactory.Get(iniPath);
#if false
            foreach (var sec in ini.Sections)
                foreach (var item in sec.Items)
                    item.Value = ApplyMacros(item.Value); 
#endif
            return ini;
        }

        public string ApplyMacros(string allText)
        {
            List<string> reported = null;
            var ret = Regex.Replace(allText, macroPattern, match =>
            {
                string macro = match.Groups[1].Value;
                string text = this.Macros.GetMacroText(macro);
                if (string.IsNullOrEmpty(text))
                {
                    text = macro;

                    if (reported == null)
                        reported = new List<string>();

                    if (!reported.Contains(macro))
                    {
                        reported.Add(macro);
                        this.Report?.Invoke($"Macro '{macro}' not found!");
                    }
                }
                return text;
            });
            return ret;
        }

        bool GetBoolExpr(Section sec, string key, bool defaultValue)
        {
            bool ret = defaultValue;
            Item itm = sec.Items.FirstOrDefault(a => a.Key == key);
            if (itm != null)
            {
                ret = this.GetBool(this.ApplyMacros(itm.Value), false);
                var items = sec.Items.ToList();
                for (int i = items.IndexOf(itm) + 1; i < items.Count; i++)
                {
                    Item itm2 = items[i];

                    if (itm2.HasKey)
                        break; // next key
                    else if (!itm2.HasValue)
                        continue; // only comment

                    string value2 = this.ApplyMacros(itm2.Value);
                    if (!ret && value2.StartsWith("OR "))
                        ret = this.GetBool(value2.Substring(3), false);
                    else if (ret && value2.StartsWith("AND "))
                        ret = this.GetBool(value2.Substring(4), false);
                    else
                        break;
                }
            }
            return ret;
        }

        bool GetBool(string expression, bool defaultValue)
        {
            bool ok = true;
            bool ret = defaultValue;
            if (!string.IsNullOrEmpty(expression))
            {
                var m = Regex.Match(expression, @$"(\bNOT\b)?\s*(\w+)(\s.*)?");
                if (m.Success)
                {
                    string value = m.Groups[2].Value;
                    if (value.ToUpper() == "TRUE")
                        ret = true;
                    else if (value.ToUpper() == "FALSE")
                        ret = false;
                    else if (value == Condition.FolderExists)
                        ret = Directory.Exists(m.Groups[3].Value.Trim());
                    else if (value == Condition.FileExists)
                        ret = File.Exists(m.Groups[3].Value.Trim());
                    else
                        ok = false;
                }
                else
                    ok = false;

                if (ok && m.Groups[1].Length != 0)
                    ret = !ret;

                if (!ok)
                    this.Report?.Invoke($"Invalid condition: {expression}");
            }
            return ret;
        }

        public IEnumerable<int> GetArguments(string args)
        {
            var mm = Regex.Matches(args, argumentPattern);
            foreach (Match m in mm)
            {
                yield return int.Parse(m.Groups[1].Value);
            }
        }
    }
}
