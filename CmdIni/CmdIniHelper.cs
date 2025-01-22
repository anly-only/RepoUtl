using csutl.ini;
using System;
using System.IO;

namespace cmd_ini
{
    public static class CmdIniHelper
    {
        public static string Separator => "[separator]\r\n";


        public static void CreateIniFile(string path, params string[] texts)
        {
            string text = string.Join("\r\n", texts);
            File.WriteAllText(path, text);
        }

        public static string GetExploreRootText()
        {
            var ini = IniFactory.Get();

            var sec = ini.GetSection("");

            sec = ini.GetSection("Explore root");
            sec.Set(CmdIni.Key.File, $"<root>");
            sec.Set(CmdIni.Key.Visible, $"{CmdIni.Condition.FolderExists} <root>");
            sec.Set(CmdIni.Key.Enabled, $"{CmdIni.Condition.FolderExists} <root>");

            sec = ini.GetSection("Documents");
            sec.Set(CmdIni.Key.File, $"<{Environment.SpecialFolder.MyDocuments}>");
            sec.Set(CmdIni.Key.Enabled, $"true");

            sec = ini.GetSection("Downloads");
            sec.Set(CmdIni.Key.File, $"<374DE290-123F-4565-9164-39C4925E467B>");

            sec = ini.GetSection("Search *.sln");
            sec.Set(CmdIni.Key.File, @"search-ms:query=.sln&crumb=location:<root>\&");

            string text = string.Join("\r\n", ini.GetText());
            return text;
        }

        public static string GetBeyondCompareText()
        {
            return @"
[Compare Files]
   File = C:\Program Files\Beyond Compare 4\BCompare.exe
   Args = ""<1>"" ""<2>""
   Visible = file_exists <1>
         AND file_exists <2>

[Compare Folders]
   File = C:\Program Files\Beyond Compare 4\BCompare.exe
   Args = ""<1>"" ""<2>""
   Visible = folder_exists <1>
         AND folder_exists <2>
";
        }

        public static string GetExploreIniText(string path)
        {
            var ini = IniFactory.Get();

            var sec = ini.GetSection("Explore INI");
            sec.Set(CmdIni.Key.File, $"explorer.exe");
            sec.Set(CmdIni.Key.Args, $"/select, {path}");

            sec = ini.GetSection("Edit INI");
            sec.Set(CmdIni.Key.File, $"{path}");
            sec.Set(CmdIni.Key.Visible, $"{CmdIni.Condition.FileExists} {path}");
            sec.Set(CmdIni.Key.Enabled, $"{CmdIni.Condition.FileExists} {path}");
            sec.Set(CmdIni.Key.WaitForExit, "true");

            string text = string.Join("\r\n", ini.GetText());
            return text;
        }

        public static string GetExampleText()
        {
            return @"
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
";
        }

        public static string GetGitCommandsText()
        {
            return
                @"
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
";
        }
    }
}
