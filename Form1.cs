using csutl;
using forms_ex;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RepoUtl
{
    public partial class Form1 : Form
    {
        int minWidth;
        ToolTip toolTip = new ToolTip();
        const string basePostfix = "-base";
        DocumentWatcher lastCopyWatcher = new DocumentWatcher();

        string Root
        {
            get => cbRepo.Text;
            set => cbRepo.Text = value;
        }

        public Form1()
        {
            InitializeComponent();
            lastCopyWatcher.OnFileChanged += this.LastCopyWatcher_OnFileChanged;
        }

        void Form1_Load(object sender, EventArgs e)
        {
#if !DEBUG
            //bnCorrectMergeInfo.Visible = false;
            //bnCorrectRevisions.Visible = false;
#endif
            toolTip.SetToolTip(bnExplore, TEXT.CommandsTooltip);
            toolTip.SetToolTip(bnMergeChanges, TEXT.CommandsTooltip);
            minWidth = Width;
            cbRepo.MouseWheel += (c, ee) => (c as Control).Zoom_MouseWheel(ee);
            cbPostfix.MouseWheel += (c, ee) => (c as Control).Zoom_MouseWheel(ee);
            tbReport.MouseWheel += (c, ee) => (c as Control).Zoom_MouseWheel(ee);
            LoadProperties();
            ui_update();
            ScanGitWorktrees();
            ShowHelp();
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveProperties();
        }

        void ui_update()
        {
            try
            {
                var kind = RepoBase.GetRepoKind(Root, out string wc);

                bnIgnoreUnversioned.Enabled = kind == RepoKind.Svn;
                //bnCorrectMergeInfo.Enabled = kind == RepoKind.Svn;
                //bnCorrectRevisions.Enabled = kind == RepoKind.Svn;

                bnWorkTree.Enabled = kind == RepoKind.Git;

                bnChanges.Enabled = kind != RepoKind.None;
                bnMergeChanges.Enabled = kind != RepoKind.None;

                WatcherUpdate();
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        void WatcherUpdate()
        {
            var last = GetTargetRoot(Root, true);
            lastCopyWatcher.Start(last);

            if (Directory.Exists(last))
                bnMergeChanges.Text = TEXT.MergeModified;
            else
                bnMergeChanges.Text = TEXT.CopyModified;
        }

        void LastCopyWatcher_OnFileChanged()
        {
            this.Invoke(() => ui_update());
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
                    cbRepo.Items.Add(item);

            if (Properties.Settings.Default.Postfix == null)
                Properties.Settings.Default.Postfix = new System.Collections.Specialized.StringCollection();

            foreach (var item in Properties.Settings.Default.Postfix.Cast<string>().Distinct().Take(10))
                cbPostfix.Items.Add(item);

            if (cbRepo.Items.Count != 0)
                cbRepo.SelectedIndex = 0;

            Properties.Settings.Default.Paths.Clear();
            Properties.Settings.Default.Postfix.Clear();

            if (!Properties.Settings.Default.FormUI.IsEmpty())
            {
                ControlEx.ReadUI(this.GetControls(true), Properties.Settings.Default.FormUI);
                tableLayoutPanel1.Dock = DockStyle.None; // set correct control positions and sizes
                tableLayoutPanel1.Dock = DockStyle.Fill;
            }
        }

        void SaveProperties()
        {
            foreach (var item in cbRepo.Items.Cast<RepoComboItem>())
                Properties.Settings.Default.Paths.Add(item.SaveToString());

            foreach (var item in cbPostfix.Items)
                Properties.Settings.Default.Postfix.Add(item as string);

            Properties.Settings.Default.FormUI = ControlEx.SaveUI(this, cbPostfix, cbRepo, tbReport);

            Properties.Settings.Default.Save();
        }

        RepoComboItem FindRepo(string path)
        {
            var x = cbRepo.Items.Cast<RepoComboItem>().FirstOrDefault(a => a.RepoPath == path);
            return x;
        }

        void Report(string text) => UTL.Report(tbReport, text);

        void EnumChanges(Action<RepoItem> back)
        {
            IRepo repo = RepoBase.GetRepo(Root, Report);
            try
            {
                int count = 0;
                repo.EnumChanges(a =>
                {
                    back.Invoke(a);
                    count++;
                });
                Report(count.ToString() + " files");
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        void Status_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (!Directory_Exists(Root))
                    return;

                EnumChanges(a =>
                {
                    string text = a.Path.Substring(Root.Length + 1);
                    if (a.Status == ItemStatus.Unversioned)
                        text += "  unversioned";
                    Report(text);
                });

                var last = GetLastTargetRoot(Root);
                if (Directory.Exists(last))
                    Report($"Last copy: {last}");
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void CmCopy_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var last = GetTargetRoot(Root, true);
            bnCopyChanges.Enabled = Directory.Exists(last);
        }

        void bnCopyChanges_Click(object sender, EventArgs e) => Copy(false);

        void bnMergeChanges_Click(object sender, EventArgs e) => Copy(true);

        // changes are merged into previous folder, if merge == true and postfix not changed
        void Copy(bool merge)
        {
            if (!Directory_Exists(Root))
                return;

            try
            {
                string root2 = GetTargetRoot(Root, merge);
                string root3 = root2 + " " + basePostfix;

                Report($"");
                Report($"{root2}");

                IRepo repo = RepoBase.GetRepo(Root, Report);

                Cursor.Current = Cursors.WaitCursor;
                bool copyOriginal = cbCopyOriginal.Checked;
                int count = 0;
                repo.EnumChanges(item =>
                {
                    Report(item.Path.Substring(Root.Length + 1));
                    if (File.Exists(item.Path))
                    {
                        string s = $"{root2}{item.Path.Substring(Root.Length)}";
                        Directory.CreateDirectory(Path.GetDirectoryName(s));
                        File.Copy(item.Path, s, true);
                    }

                    if (copyOriginal && item.Status == ItemStatus.Modified)
                    {
                        string s = $"{root3}{item.Path.Substring(Root.Length)}";
                        Directory.CreateDirectory(Path.GetDirectoryName(s));
                        repo.CopyOriginalTo(item, s);
                    }
                    count++;
                });
                Report(count.ToString() + " files");
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                ui_update();
            }
        }

        const int startNum = 1;

        string GetTargetRoot(string root, bool merge)
        {
            var postfix = cbPostfix.Text;
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
            tbReport.Clear();
        }

        void bnIgnoreUnversioned_Click(object sender, EventArgs e)
        {
            try
            {
                IRepo repo = RepoBase.GetRepo(Root, Report);
                repo.IgnoreUnversioned();
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        void bnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                if (UTL.Browse(Root, out var s, TEXT.SelectRepo))
                {
                    var path = RepoBase.GetWorkingCopyFolder(s);
                    RepoComboItem repo = FindRepo(path);
                    if (repo == null)
                        repo = new RepoComboItem(path);
                    cbRepo.SelectItem(repo);
                    ui_update();
                }
            }
            catch (Exception ee)
            {
                Report(ee.Message);
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

                    Root = path;
                }
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        void bnExplore_Click(object sender, EventArgs e)
        {
            var ini = SettingsIni.GetIni();
            var section = ini.GetSection(SettingsIni.Key.ExploreRepoSection);

            if (section.Bool(SettingsIni.Key.CloseDuplicates, true))
                UTL.CloseDuplicatedExplorerWindows();

            UTL.Explore(Root,
                section.Bool(SettingsIni.Key.Select, true),
                section.Bool(SettingsIni.Key.TrySingleWindow, true));
        }

        void bnCorrectMergeInfo_Click(object sender, EventArgs e)
        {
            var text = Clipboard.GetText();
            var ret = SvnMergeInfo.MinimizeMergeInfo(text);
            ReportResult(text, ret);
        }


        void bnCorrectRevisions_Click(object sender, EventArgs e)
        {
            var text = Clipboard.GetText();
            var ret = SvnMergeInfo.MinimizeRevisions(text);
            ReportResult(text, ret);
        }

        void ReportResult(string text, string ret)
        {
            if (ret.IsEmpty())
            {
                Report("Clipboard have text in wrong format.");
            }
            else if (ret != text)
            {
                Clipboard.SetText(ret);
                Report("The result in clipboard.");
            }
            else
            {
                Report("No changes.");
            }
        }

        bool Directory_Exists(string folder, bool report = true)
        {
            bool yes = Directory.Exists(folder);

            if (!yes && report)
                Report($"Folder is not exists: {folder}");

            return yes;
        }

        void bnWorkTree_Click(object sender, EventArgs e)
        {
            try
            {
                if (RepoBase.GetRepoKind(Root, out var wc) == RepoKind.Git)
                {
                    var f = new WorkTreeForm();
                    f.uc.OnReport = Report;
                    f.uc.RepoWorkingCopyPath = Root;
                    f.uc.OnWorktreePathChanged += this.Uc_OnWorktreePathChanged;
                    f.ShowDialog();
                    ui_update();
                }
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        void ScanGitWorktrees()
        {
            var kind = RepoBase.GetRepoKind(Root, out string wc);
            if (kind == RepoKind.Git)
            {
                var ini = SettingsIni.GetIni();
                var sec = ini.GetSection(SettingsIni.Key.Git);
                if (sec.Bool(SettingsIni.Key.AutoScanWorktrees, true))
                {
                    var repo = RepoBase.GetRepo(wc, Report) as IRepoGit;
                    var wts = repo.GetWorkTrees();
                    foreach (var item in wts)
                    {
                        string s = RepoGit.GetWorktreePath(wc, item.Name);
                        if (!cbRepo.Items.Cast<RepoComboItem>().Any(a => a.RepoPath == s))
                            cbRepo.Items.Add(new RepoComboItem(s));
                    }
                }
            }
        }

        void Uc_OnWorktreePathChanged(string path)
        {
            if (path != null)
            {
                RepoComboItem repo = FindRepo(path);
                if (repo == null)
                    repo = new RepoComboItem(path);
                cbRepo.SelectItem(repo);
            }
        }

        void tbLocalPath_TextUpdate(object sender, EventArgs e)
        {
            ui_update();
        }

        void cbRepo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbRepo.SelectedIndex > 0)
                {
                    cbRepo.SelectItem(cbRepo.SelectedItem); // move item to begin of list
                }
                else
                {
                    var x = cbRepo.SelectedItem as RepoComboItem;
                    cbPostfix.SelectItem(x.Postfix);
                    ui_update();
                }
                ScanGitWorktrees();
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        void cbPostfix_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbPostfix.SelectedIndex > 0)
                {
                    cbPostfix.SelectItem(cbPostfix.SelectedItem); // move item to begin of list
                }
                else
                {
                    OnPostfixChanged();
                }
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        void cbPostfix_TextChanged(object sender, EventArgs e) => OnPostfixChanged();

        void OnPostfixChanged()
        {
            try
            {
                var repo = cbRepo.SelectedItem as RepoComboItem;
                if (repo != null)
                    repo.Postfix = cbPostfix.Text;
                ui_update();
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            this.ProcessKeyPreviewHandler(ref m, () => WindowState = FormWindowState.Minimized);
            return base.ProcessKeyPreview(ref m);
        }

        void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (Size.Width < minWidth)
                Width = minWidth;

            cbRepo.SelectionLength = 0; // avoid select-all side effect
            cbPostfix.SelectionLength = 0;
        }

        void Form1_Shown(object sender, EventArgs e)
        {
            cbRepo.SelectionLength = 0; // avoid select-all side effect
            cbPostfix.SelectionLength = 0;
        }

        #region Commands
        IEnumerable<Cmd> GetCommands() => (new CmdIni(Root, Report)).GetCommands(CmdIni.IniFileFullName);

        private void cm_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            ClearCm();
            cm.Items.Add("dummy");
            if (cm.Tag != null)
            {
                ExecuteCmd((Cmd)cm.Tag);
                cm.Tag = null;
            }
        }

        private void ClearCm()
        {
            foreach (ToolStripItem item in cm.Items)
                item.MouseDown -= C_MouseDown;
            cm.Items.Clear();
        }

        private void cm_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ClearCm();
            bool prevSeparator = true;
            foreach (Cmd cmd in GetCommands())
            {
                if (cmd.IsSeparator)
                {
                    if (!prevSeparator)
                        cm.Items.Add(new ToolStripSeparator());
                    prevSeparator = true;
                }
                else
                {
                    var c = cm.Items.Add(cmd.Name);
                    c.Enabled = cmd.Enabled;
                    c.Tag = cmd;
                    c.MouseDown += C_MouseDown;
                    prevSeparator = false;
                }
            }
        }

        private void C_MouseDown(object sender, MouseEventArgs e)
        {
            var c = (ToolStripItem)sender;
            var cmd = (Cmd)c.Tag;
            cm.Tag = cmd;
        }

        private void ExecuteCmd(Cmd cmd)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = cmd.File;
                p.StartInfo.Arguments = cmd.Args;
                p.StartInfo.UseShellExecute = true;
                p.Start();

                if (cmd.WaitForExit)
                {
                    tableLayoutPanel1.Enabled = false;
                    p.WaitForExit();
                }
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
            finally
            {
                tableLayoutPanel1.Enabled = true;
            }
        }

        #endregion

        private void cbPostfix_Leave(object sender, EventArgs e)
        {
            int ix = cbPostfix.Items.IndexOf(cbPostfix.Text);
            if (ix != 0)
            {
                var s = cbPostfix.Text;
                if (ix != -1)
                    cbPostfix.Items.RemoveAt(ix);
                cbPostfix.Items.Insert(0, s);
                cbPostfix.SelectedIndex = 0;
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
                    Report(help);
                }
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }
    }
}
