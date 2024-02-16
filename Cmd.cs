using csutl;
using csutl.ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace RepoUtl
{
    class Cmd
    {
        public string Name { get; set; }
        public string File { get; set; }
        public string Args { get; set; }
        public bool Enabled { get; set; } = true;
        public bool WaitForExit { get; set; }
        public bool IsSeparator { get; set; }
    }

    class CmdIni
    {
        internal const string IniName = "commands.ini";

        class Key
        {
            internal const string File = "File";
            internal const string Args = "Args";
            internal const string Visible = "Visible";
            internal const string Enabled = "Enabled";
            internal const string WaitForExit = "WaitForExit";
            internal const string OR = "OR";
            internal const string AND = "AND";
            internal const string NOT = "NOT";
        }

        class Condition
        {
            internal const string FolderExists = "folder_exists";
            internal const string FileExists = "file_exists";
        }

        class Macro  // used in <> scope, like: <macro>
        {
            internal const string Root = "root";
            internal const string LastTargetRoot = "last_target_root";
            internal const string LastTargetRootBase = "last_target_root_base";
        }

        //const string guidPattern = "<([0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12})>";
        const string pattern = "<((?:\\w|-)+)>";

        string RootPath { get; }
        Action<string> Report { get; }

        internal static string IniFileFullName
        {
            get
            {
                var ini = Path.Combine(App.AddDataFolder, IniName);
                if (!File.Exists(ini))
                    CreateIniFile(ini);

                return ini;
            }
        }


        internal CmdIni(string rootPath, Action<string> Report)
        {
            this.RootPath = rootPath;
            this.Report = Report;
        }

        csutl.ini.IniFile GetIni(string iniPath)
        {
            var ini = csutl.ini.IniFactory.Get(iniPath);
            foreach (var sec in ini.Sections)
                foreach (var item in sec.Items)
                    item.Value = ApplyMacros(item.Value);
            return ini;
        }

        internal IEnumerable<Cmd> GetCommands(string iniPath)
        {
            var ini = GetIni(iniPath);

            foreach (var section in ini.Sections.Where(a => !a.Name.IsEmpty()))
            {
                var c = new Cmd();
                if (GetBoolExpr(section, Key.Visible, true))
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
                        c.Enabled = GetBoolExpr(section, Key.Enabled, true);
                        c.WaitForExit = GetBoolExpr(section, Key.WaitForExit, false);
                    }
                    yield return c;
                }
            }
        }

        string ApplyMacros(string text)
        {
            var ret = Regex.Replace(text, pattern, m =>
            {
                var x = GetReplace(m.Groups[1].Value);
                if (x.IsEmpty())
                {
                    x = m.Value;
                    Report?.Invoke($"Macro '{x}' not found!");
                }
                return x;
            });

            return ret;
        }

        string GetReplace(string macro)
        {
            string ret = null;
            if (macro == Macro.Root)
            {
                ret = RootPath;
            }
            else if (macro == Macro.LastTargetRoot)
            {
                ret = Form1.GetLastTargetRoot(RootPath);
            }
            else if (macro == Macro.LastTargetRootBase)
            {
                ret = Form1.GetLastTargetRootBase(RootPath);
            }
            else if (Enum.TryParse<Environment.SpecialFolder>(macro, out var value))
            {
                ret = Environment.GetFolderPath(value);
            }
            else if (Guid.TryParse(macro, out var guid))
            {
                ret = GetGuidPath(guid);
            }
            return ret;
        }

        bool GetBoolExpr(Section sec, string key, bool defaultValue)
        {
            bool ret = defaultValue;
            Item itm = sec.Items.FirstOrDefault(a => a.Key == key);
            if (itm != null)
            {
                ret = GetBool(itm.Value, false);
                var items = sec.Items.ToList();
                for (int i = items.IndexOf(itm) + 1; i < items.Count; i++)
                {
                    Item itm2 = items[i];

                    if (itm2.HasKey)
                        break; // next key
                    else if (!itm2.HasValue)
                        continue; // only comment

                    if (!ret && itm2.Value.StartsWith("OR "))
                        ret = GetBool(itm2.Value.Substring(3), false);
                    else if (ret && itm2.Value.StartsWith("AND "))
                        ret = GetBool(itm2.Value.Substring(4), false);
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
            if (!expression.IsEmpty())
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
                    Report?.Invoke($"Invalid condition: {expression}");
            }
            return ret;
        }

        internal static void CreateIniFile(string path)
        {
            var ini = csutl.ini.IniFactory.Get();

            var sec = ini.GetSection("");

            sec = ini.GetSection("Explore root");
            sec.Set(Key.File, $"<{Macro.Root}>");
            sec.Set(Key.Visible, $"{Condition.FolderExists} <{Macro.Root}>");
            sec.Set(Key.Enabled, $"{Condition.FolderExists} <{Macro.Root}>");

            sec = ini.GetSection("Documents");
            sec.Set(Key.File, $"<{Environment.SpecialFolder.MyDocuments}>");
            sec.Set(Key.Enabled, $"true");

            sec = ini.GetSection("Downloads");
            sec.Set(Key.File, $"<374DE290-123F-4565-9164-39C4925E467B>");

            sec = ini.GetSection("Search *.sln");
            sec.Set(Key.File, @"search-ms:query=*.sln&crumb=location:<root>\&");

            ini.GetSection("Empty section is separator");

            sec = ini.GetSection("Explore INI");
            sec.Set(Key.File, $"explorer.exe");
            sec.Set(Key.Args, $"/select, <LocalApplicationData>\\RepoUtl\\{IniName}");

            sec = ini.GetSection("Edit INI");
            var x = $"<LocalApplicationData>\\RepoUtl\\{IniName}";
            sec.Set(Key.File, $"{x}");
            sec.Set(Key.Visible, $"{Condition.FileExists} {x}");
            sec.Set(Key.Enabled, $"{Condition.FileExists} {x}");
            sec.Set(Key.WaitForExit, "true");

            ini.Save(path);

            string text = File.ReadAllText(path);
            text = GitCommands() + text;
            File.WriteAllText(path, text);
        }

        static string GitCommands()
        {
            return
                @"
[Example Menu] // this is only example
   File = C:\Windows  // the command to execute

   Visible = NOT TRUE      // first operand. every operand must be on separate line
             OR NOT FALSE  // second operand

   Enabled =    folder_exists C:\Windows  // if folder exists
        AND NOT file_exists X:\Abc.txt    // and file not exists
        AND TRUE                          // and true (really no sense write this, but for examle only)
        OR FALSE                          // or false (no sense too)
   
   // The priority of AND/OR is ignored: 
   //   result of every line is simply combined with the result of the next line,
   //   if the next line exists and the result still not clear). 


[separator]

[Log]
   File = TortoiseGitProc.exe
   Args = /command:log /path:""<root>""
   Visible = file_exists <root>\.git    // file exists in worktree
          OR folder_exists <root>\.git  // folder exists in non-bare repo
[Diff]
   File = TortoiseGitProc.exe
   Args = /command:repostatus /path:""<root>""
   Visible = file_exists <root>\.git
          OR folder_exists <root>\.git
[Commit]
   File = TortoiseGitProc.exe
   Args = /command:commit /path:""<root>""
   Visible = file_exists <root>\.git
          OR folder_exists <root>\.git
[Revert]
   File = TortoiseGitProc.exe
   Args = /command:revert /path:""<root>""
   Visible = file_exists <root>\.git
          OR folder_exists <root>\.git

[separator]

[Pull]
   File = TortoiseGitProc.exe
   Args = /command:pull /path:""<root>""
   Visible = file_exists <root>\.git
          OR folder_exists <root>\.git
[Push]
   File = TortoiseGitProc.exe
   Args = /command:push /path:""<root>""
   Visible = file_exists <root>\.git
          OR folder_exists <root>\.git
[Merge]
   File = TortoiseGitProc.exe
   Args = /command:merge /path:""<root>""
   Visible = file_exists <root>\.git
          OR folder_exists <root>\.git

[separator]

";
        }

        static string GetGuidPath(Guid guid)
        {
            string ret = null;
            try
            {
                ret = SHGetKnownFolderPath(guid, 0);
            }
            catch
            {
            }
            return ret;
        }

        [DllImport("shell32",
            CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        static extern string SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guid, uint dwFlags,
            int hToken = 0);
    }
}
