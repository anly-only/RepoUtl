using System;
using System.IO;

namespace RepoUtl
{
    static class TEXT
    {
        internal const string MergeModified = "Merge modified";
        internal const string CopyModified = "Copy modified";
        public const string SelectRepo = "Please, select the working copy folder of a GIT or SVN repository";
        public const string SelectGitRepo = "Please, select the working copy folder of a GIT repository";
        public const string CommandsTooltip = "Right mouse click for additional commands";
        public const string Help =
@"Here you can read a short help how to get started with this application.

(1)
First you have to select (in the combo at the top) a working copy folder of a SVN or a GIT repository.
You can click on 'Select...' button for this.

(2)
'Copy modified'  or 'Merge modified' button.
Button make a copy of modified files or merge modified files with last copy of files. 
The target folder is (automatically created) sibling folder of your working copy.
The target folder name is extended by user defined postfix ('modified' by default).
If 'base' is checked, the not modified versions (base) of modified files are coped too, into a separate sibling folder.
To change the target folder you can change the postfix, or use context menu of 'Merge modified' button.

(3)
'Git: Worktrees...' button.
Here you can add or remove worktrees.
The worktree name can be selected from existing worktreess or from the name of a branch.

(4)
Note, that font of some controls can be zoomed by using of Control + MouseWheel.

(5)
Via context menu of 'Explore...' button you can execute or cutomize some commands and settings defined in INI files.
Use context menu 'Explore INI' to open the folder where the INI files are located.

(6)
Set 'ShowHelpAtStartup = false' in settings.ini to avoid displaing of this message at startup.
";
    }

    static class App
    {
        internal static string AddDataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RepoUtl");
    }
}
