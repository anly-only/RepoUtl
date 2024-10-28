using cmd_ini;
using cmd_ini.Forms;
using csutl;
using forms_ex;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RepoUtl
{
    public partial class Form1 : Form
    {
        int minWidth => bnExplore.Right + 20;

        ToolTip toolTip = new ToolTip();
        const string basePostfix = "-base";
        DocumentWatcher lastCopyWatcher = new DocumentWatcher();
        TextBox tbBranch;
        CmdIniMenu cmm;

        string Root
        {
            get => this.cbRepo.Text;
            set => this.cbRepo.Text = value;
        }

        public Form1()
        {
            this.InitializeComponent();

            this.tbBranch = new TextBox();
            this.tableLayoutPanel1.Controls.Add(this.tbBranch, 0, 1);
            this.tableLayoutPanel1.SetColumnSpan(this.tbBranch, 2);
            this.tbBranch.Location = new Point(3, 33);
            this.tbBranch.Name = "lbBranch";
            this.tbBranch.Size = new Size(290, 29);
            this.tbBranch.TabIndex = 1;
            this.tbBranch.Text = "Svn: ignore unversioned folders";
            this.tbBranch.ReadOnly = true;
            this.tbBranch.BorderStyle = BorderStyle.None;

            this.cmm = new CmdIniMenu(this.cm);

            this.lastCopyWatcher.OnFileChanged += this.LastCopyWatcher_OnFileChanged;
            this.Activated += this.Form1_Activated;
        }

        void Form1_Load(object sender, EventArgs e)
        {
#if !DEBUG
            //bnCorrectMergeInfo.Visible = false;
            //bnCorrectRevisions.Visible = false;
#endif
            this.toolTip.SetToolTip(this.bnExplore, TEXT.CommandsTooltip);
            this.toolTip.SetToolTip(this.bnMergeChanges, TEXT.CommandsTooltip);
            this.cbRepo.MouseWheel += (c, ee) => (c as Control).Zoom_MouseWheel(ee);
            this.cbPostfix.MouseWheel += (c, ee) => (c as Control).Zoom_MouseWheel(ee);
            this.tbReport.MouseWheel += (c, ee) => (c as Control).Zoom_MouseWheel(ee);
            this.tbBranch.MouseWheel += (c, ee) => (c as Control).Zoom_MouseWheel(ee);
            this.LoadProperties();
            this.ui_update();
            this.ScanGitWorktrees();
            this.ShowHelp();
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveProperties();
        }

        void Form1_Activated(object sender, EventArgs e)
        {
            this.tbBranch.Text = string.Empty;
            this.ui_update();
        }

        void ui_update()
        {
            try
            {
                var kind = RepoBase.GetRepoKind(this.Root, out string wc);

                this.bnIgnoreUnversioned.Visible = kind == RepoKind.Svn;
                this.tbBranch.Visible = kind == RepoKind.Git;

                if (kind == RepoKind.Git && string.IsNullOrEmpty(this.tbBranch.Text))
                {
                    RepoGit repo = RepoBase.GetRepo(this.Root, this.Report) as RepoGit;
                    this.tbBranch.Text = repo.CurrentBranch;
                }

                //bnCorrectMergeInfo.Enabled = kind == RepoKind.Svn;
                //bnCorrectRevisions.Enabled = kind == RepoKind.Svn;

                this.bnWorkTree.Enabled = kind == RepoKind.Git;

                this.bnChanges.Enabled = kind != RepoKind.None;
                this.bnMergeChanges.Enabled = kind != RepoKind.None;

                this.WatcherUpdate();
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        void WatcherUpdate()
        {
            var last = this.GetTargetRoot(this.Root, true);
            this.lastCopyWatcher.Start(last);

            if (Directory.Exists(last))
                this.bnMergeChanges.Text = TEXT.MergeModified;
            else
                this.bnMergeChanges.Text = TEXT.CopyModified;
        }

        void LastCopyWatcher_OnFileChanged()
        {
            this.Invoke(() => this.ui_update());
        }


        void LoadProperties()
        {
            if (Properties.Settings.Default.Paths == null)
                Properties.Settings.Default.Paths = new System.Collections.Specialized.StringCollection();

            var repos = Properties.Settings.Default.Paths.Cast<string>()
                .Where(a => a != null)
                .Select(a => new RepoComboItem(a));

            foreach (var item in repos)
                if (Directory.Exists(item.RepoPath))
                    this.cbRepo.Items.Add(item);

            if (Properties.Settings.Default.Postfix == null)
                Properties.Settings.Default.Postfix = new System.Collections.Specialized.StringCollection();

            foreach (var item in Properties.Settings.Default.Postfix.Cast<string>().Distinct().Take(10))
                this.cbPostfix.Items.Add(item);

            if (this.cbRepo.Items.Count != 0)
                this.cbRepo.SelectedIndex = 0;

            Properties.Settings.Default.Paths.Clear();
            Properties.Settings.Default.Postfix.Clear();

            if (!Properties.Settings.Default.FormUI.IsEmpty())
            {
                ControlEx.ReadUI(this.GetControls(true), Properties.Settings.Default.FormUI);
                this.tableLayoutPanel1.Dock = DockStyle.None; // set correct control positions and sizes
                this.tableLayoutPanel1.Dock = DockStyle.Fill;
            }
        }

        void SaveProperties()
        {
            foreach (var item in this.cbRepo.Items.Cast<RepoComboItem>())
                Properties.Settings.Default.Paths.Add(item.SaveToString());

            foreach (var item in this.cbPostfix.Items)
                Properties.Settings.Default.Postfix.Add(item as string);

            Properties.Settings.Default.FormUI = ControlEx.SaveUI(this, this.cbPostfix, this.cbRepo, this.tbReport, this.tbBranch);

            Properties.Settings.Default.Save();
        }

        RepoComboItem FindRepo(string path)
        {
            var x = this.cbRepo.Items.Cast<RepoComboItem>().FirstOrDefault(a => a.RepoPath == path);
            return x;
        }

        void Report(string text) => UTL.Report(this.tbReport, text);

        void EnumChanges(Action<RepoItem> back)
        {
            IRepo repo = RepoBase.GetRepo(this.Root, this.Report);
            try
            {
                int count = 0;
                repo.EnumChanges(a =>
                {
                    back.Invoke(a);
                    count++;
                });
                this.Report(count.ToString() + " files");
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        void Status_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (!this.Directory_Exists(this.Root))
                    return;

                this.EnumChanges(a =>
                {
                    string text = a.Path.Substring(this.Root.Length + 1);
                    if (a.Status == ItemStatus.Unversioned)
                        text += "  unversioned";
                    this.Report(text);
                });

                var last = GetLastTargetRoot(this.Root);
                if (Directory.Exists(last))
                    this.Report($"Last copy: {last}");
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void CmCopy_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var last = this.GetTargetRoot(this.Root, true);
            this.bnCopyChanges.Enabled = Directory.Exists(last);
        }

        void bnCopyChanges_Click(object sender, EventArgs e) => this.Copy(false);

        void bnMergeChanges_Click(object sender, EventArgs e) => this.Copy(true);

        // changes are merged into previous folder, if merge == true and postfix not changed
        void Copy(bool merge)
        {
            if (!this.Directory_Exists(this.Root))
                return;

            try
            {
                string root2 = this.GetTargetRoot(this.Root, merge);
                string root3 = root2 + " " + basePostfix;

                this.Report($"");
                this.Report($"{root2}");

                IRepo repo = RepoBase.GetRepo(this.Root, this.Report);

                Cursor.Current = Cursors.WaitCursor;
                bool copyOriginal = this.cbCopyOriginal.Checked;
                int count = 0;
                repo.EnumChanges(item =>
                {
                    this.Report(item.Path.Substring(this.Root.Length + 1));
                    if (File.Exists(item.Path))
                    {
                        string s = $"{root2}{item.Path.Substring(this.Root.Length)}";
                        Directory.CreateDirectory(Path.GetDirectoryName(s));
                        File.Copy(item.Path, s, true);
                    }

                    if (copyOriginal && item.Status == ItemStatus.Modified)
                    {
                        string s = $"{root3}{item.Path.Substring(this.Root.Length)}";
                        Directory.CreateDirectory(Path.GetDirectoryName(s));
                        repo.CopyOriginalTo(item, s);
                    }
                    count++;
                });
                this.Report(count.ToString() + " files");
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                this.ui_update();
            }
        }

        const int startNum = 1;

        string GetTargetRoot(string root, bool merge)
        {
            if (string.IsNullOrEmpty(root))
                return string.Empty;

            var postfix = this.cbPostfix.Text.Trim();
            if (string.IsNullOrWhiteSpace(postfix))
                postfix = "modified";

            int number = GetMaxNumber(root, out string lastPostfix);
            if (!merge || number == startNum || lastPostfix != postfix)
                ++number;

            string root2 = GetTargetRoot(root, number, postfix);
            if (!merge)
                while (Directory.Exists((root2)))
                    root2 += "-";

            return root2;
        }

        // the target root that was used last time
        internal static string GetLastTargetRoot(string root)
        {
            int number = GetMaxNumber(root, out string lastPostfix);
            var root2 = GetTargetRoot(root, number, lastPostfix);
            return root2;
        }
        internal static string GetLastTargetRootBase(string root)
        {
            return $"{GetLastTargetRoot(root)} {basePostfix}";
        }

        static string GetTargetRoot(string root, int number, string postfix)
        {
            string root2 = $"{root} ({number}) {postfix}".Trim();
            return root2;
        }

        static int GetMaxNumber(string root, out string postfix)
        {
            postfix = string.Empty;
            var rootParentDir = Path.GetDirectoryName(root);
            var rootName = Path.GetFileName(root);
            var dirs = Directory.GetDirectories(rootParentDir, rootName + "*");
            int number = startNum;
            foreach (var d in dirs)
            {
                var x = Path.GetFileName(d);
                if (!x.EndsWith(basePostfix))
                {
                    var m = Regex.Match(x, @"\((\d+)\)\s*(.+)");
                    if (m.Value != "")
                    {
                        var n = int.Parse(m.Groups[1].Value);
                        if (n > number)
                        {
                            number = n;
                            postfix = m.Groups[2].Value;
                        }
                    }
                }
            }

            return number;
        }

        void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tbReport.Clear();
        }

        void bnIgnoreUnversioned_Click(object sender, EventArgs e)
        {
            try
            {
                IRepo repo = RepoBase.GetRepo(this.Root, this.Report);
                repo.IgnoreUnversioned();
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        void bnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                if (UTL.Browse(this.Root, out var s, TEXT.SelectRepo))
                {
                    var path = RepoBase.GetWorkingCopyFolder(s);
                    RepoComboItem repo = this.FindRepo(path);
                    if (repo == null)
                        repo = new RepoComboItem(path);
                    this.cbRepo.SelectItem(repo);
                    this.ui_update();
                }
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        void tbLocalPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileDrop"))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        void tbLocalPath_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var o = e.Data.GetData("FileDrop");
                if (o is string[])
                {
                    string[] ar = o as string[];
                    string path = ar[0];

                    if (File.Exists(path))
                        path = Path.GetDirectoryName(path);

                    this.Root = path;
                }
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        void bnExplore_Click(object sender, EventArgs e)
        {
            var ini = SettingsIni.GetIni();
            var section = ini.GetSection(SettingsIni.Key.ExploreRepoSection);

            if (section.Bool(SettingsIni.Key.CloseDuplicates, true))
                UTL.CloseDuplicatedExplorerWindows();

            UTL.Explore(this.Root,
                section.Bool(SettingsIni.Key.Select, true),
                section.Bool(SettingsIni.Key.TrySingleWindow, true));
        }

        void bnCorrectMergeInfo_Click(object sender, EventArgs e)
        {
            var text = Clipboard.GetText();
            var ret = SvnMergeInfo.MinimizeMergeInfo(text);
            this.ReportResult(text, ret);
        }


        void bnCorrectRevisions_Click(object sender, EventArgs e)
        {
            var text = Clipboard.GetText();
            var ret = SvnMergeInfo.MinimizeRevisions(text);
            this.ReportResult(text, ret);
        }

        void ReportResult(string text, string ret)
        {
            if (ret.IsEmpty())
            {
                this.Report("Clipboard have text in wrong format.");
            }
            else if (ret != text)
            {
                Clipboard.SetText(ret);
                this.Report("The result in clipboard.");
            }
            else
            {
                this.Report("No changes.");
            }
        }

        bool Directory_Exists(string folder, bool report = true)
        {
            bool yes = Directory.Exists(folder);

            if (!yes && report)
                this.Report($"Folder is not exists: {folder}");

            return yes;
        }

        void bnWorkTree_Click(object sender, EventArgs e)
        {
            try
            {
                if (RepoBase.GetRepoKind(this.Root, out var wc) == RepoKind.Git)
                {
                    var f = new WorkTreeForm();
                    f.uc.OnReport = this.Report;
                    f.uc.RepoWorkingCopyPath = this.Root;
                    f.uc.OnWorktreePathChanged += this.Uc_OnWorktreePathChanged;
                    f.ShowDialog();
                    this.ui_update();
                }
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        void ScanGitWorktrees()
        {
            var kind = RepoBase.GetRepoKind(this.Root, out string wc);
            if (kind == RepoKind.Git)
            {
                var ini = SettingsIni.GetIni();
                var sec = ini.GetSection(SettingsIni.Key.Git);
                if (sec.Bool(SettingsIni.Key.AutoScanWorktrees, true))
                {
                    var repo = RepoBase.GetRepo(wc, this.Report) as IRepoGit;
                    var wts = repo.GetWorkTrees();
                    foreach (var item in wts)
                    {
                        string s = RepoGit.GetWorktreePath(wc, item.Name);
                        if (!this.cbRepo.Items.Cast<RepoComboItem>().Any(a => a.RepoPath == s))
                            this.cbRepo.Items.Add(new RepoComboItem(s));
                    }
                }
            }
        }

        void Uc_OnWorktreePathChanged(string path)
        {
            if (path != null)
            {
                RepoComboItem repo = this.FindRepo(path);
                if (repo == null)
                    repo = new RepoComboItem(path);
                this.cbRepo.SelectItem(repo);
            }
        }

        void tbLocalPath_TextUpdate(object sender, EventArgs e)
        {
            this.ui_update();
        }

        void cbRepo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.tbBranch.Text = string.Empty;
                if (this.cbRepo.SelectedIndex > 0)
                {
                    this.cbRepo.SelectItem(this.cbRepo.SelectedItem); // move item to begin of list
                }
                else
                {
                    var x = this.cbRepo.SelectedItem as RepoComboItem;
                    this.cbPostfix.SelectItem(x.Postfix);
                    this.ui_update();
                }
                this.ScanGitWorktrees();
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        void cbPostfix_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cbPostfix.SelectedIndex > 0)
                {
                    this.cbPostfix.SelectItem(this.cbPostfix.SelectedItem); // move item to begin of list
                }
                else
                {
                    this.OnPostfixChanged();
                }
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        void cbPostfix_TextChanged(object sender, EventArgs e) => this.OnPostfixChanged();

        void OnPostfixChanged()
        {
            try
            {
                var repo = this.cbRepo.SelectedItem as RepoComboItem;
                if (repo != null)
                    repo.Postfix = this.cbPostfix.Text;
                this.ui_update();
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            this.ProcessKeyPreviewHandler(ref m, () => this.WindowState = FormWindowState.Minimized);
            return base.ProcessKeyPreview(ref m);
        }

        void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.Size.Width < this.minWidth)
                this.Width = this.minWidth;

            this.cbRepo.SelectionLength = 0; // avoid select-all side effect
            this.cbPostfix.SelectionLength = 0;
        }

        void Form1_Shown(object sender, EventArgs e)
        {
            this.cbRepo.SelectionLength = 0; // avoid select-all side effect
            this.cbPostfix.SelectionLength = 0;
        }

        #region Commands
        IEnumerable<Cmd> GetCommands()
        {
            var m = new CmdMacros();
            m.SetMacro("root", this.Root);
            var file = Path.Combine(App.AddDataFolder, "commands.ini");
            if (!File.Exists(file))
                CreateIniFile(file);
            var ini = new CmdIni(file, m, this.Report);
            return ini.GetCommands();
        }

        static void CreateIniFile(string path)
        {
            CmdIniHelper.CreateIniFile(
                path,
                CmdIniHelper.GetExampleText(),
                CmdIniHelper.Separator,
                CmdIniHelper.GetGitCommandsText(),
                CmdIniHelper.Separator,
                CmdIniHelper.GetExploreRootText(),
                CmdIniHelper.Separator,
                CmdIniHelper.GetExploreIniText(path),
                ""
                );
        }

        private void cm_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int ix = this.cm.Items.Cast<ToolStripItem>().IndexOf(a => a.Text == "--dummy--");
            if (ix != -1)
                this.cm.Items.RemoveAt(ix);

            this.cmm.cm_Opening(sender, this.GetCommands());
        }

        private void cm_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            if (this.cm.Tag is Cmd cmd)
            {
                this.cmm.cm_Closed(cmd, Enumerable.Empty<string>(), this.tableLayoutPanel1, this.Report);
            }

            if (this.cm.Items.Count == 0)
                this.cm.Items.Add(new ToolStripButton("--dummy--"));
        }

        #endregion

        private void cbPostfix_Leave(object sender, EventArgs e)
        {
            int ix = this.cbPostfix.Items.IndexOf(this.cbPostfix.Text);
            if (ix != 0)
            {
                var s = this.cbPostfix.Text;
                if (ix != -1)
                    this.cbPostfix.Items.RemoveAt(ix);
                this.cbPostfix.Items.Insert(0, s);
                this.cbPostfix.SelectedIndex = 0;
            }
        }

        void ShowHelp()
        {
            try
            {
                var ini = SettingsIni.GetIni();
                var sec = ini.GetSection(SettingsIni.Key.Common);
                if (sec[SettingsIni.Key.ShowHelpAtStartup, false].Bool)
                {
                    var helpfile = Path.Combine(App.AddDataFolder, "help.txt");
                    if (!File.Exists(helpfile))
                        File.WriteAllText(helpfile, TEXT.Help);

                    var help = File.ReadAllText(helpfile);
                    this.Report(help);
                }
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }
    }
}
