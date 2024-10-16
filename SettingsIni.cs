using System.IO;

namespace RepoUtl
{
    internal class SettingsIni
    {
        internal const string IniName = "settings.ini";

        internal class Key
        {
            // sections
            internal const string Common = "Common";
            internal const string ExploreRepoSection = "ExploreRepo";
            internal const string ExploreWorktreeSection = "ExploreWorktree";
            internal const string Git = "GIT";

            // keys
            internal const string Select = "Select";
            internal const string TrySingleWindow = "TrySingleWindow";
            internal const string CloseDuplicates = "CloseDuplicates";
            internal const string AutoScanWorktrees = "AutoScanWorktrees";
            internal const string ShowHelpAtStartup = "ShowHelpAtStartup";
        }


        static string IniFileFullName
        {
            get
            {
                var ini = Path.Combine(App.AddDataFolder, IniName);
                if (!File.Exists(ini))
                    CreateIniFile(ini);

                return ini;
            }
        }

        SettingsIni()
        {
        }

        internal static csutl.ini.IniFile GetIni()
        {
            var ini = csutl.ini.IniFactory.Get(IniFileFullName);
            return ini;
        }

        static void CreateIniFile(string path)
        {
            var ini = csutl.ini.IniFactory.Get();

            var sec = ini[Key.Common];
            sec.Set(Key.ShowHelpAtStartup, "true", "true - show help at startup");

            sec = ini.GetSection(Key.ExploreRepoSection, "Explore repo command");
            sec.Set(Key.Select, "true", "true - select folder, false - open folder");
            sec.Set(Key.TrySingleWindow, "true", "try to use already opened explorer window");
            sec.Set(Key.CloseDuplicates, "true", "additionally, close all duplicates between opened explorer windows");

            sec = ini.GetSection(Key.ExploreWorktreeSection, "Explore worktree command");
            sec.Set(Key.Select, "true", "true - select folder, false - open folder");
            sec.Set(Key.TrySingleWindow, "true", "try to use already opened explorer window");
            sec.Set(Key.CloseDuplicates, "true", "additionally, close all duplicates between opened explorer windows");

            sec = ini.GetSection(Key.Git, "Git settings");
            sec.Set(Key.AutoScanWorktrees, "true", "Automatically scan worktrees and add them into combo list");

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            ini.Save(path);
        }
    }
}
