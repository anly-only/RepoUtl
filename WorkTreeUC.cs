using forms_ex;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RepoUtl
{
    public partial class WorkTreeUC : UserControl
    {
        public WorkTreeUC()
        {
            this.InitializeComponent();
        }

        void WorkTreeUC_Load(object sender, EventArgs e)
        {
            this.tbRepo.MouseWheel += (c, ee) => (c as Control).Zoom_MouseWheel(ee);
            this.tbWorkTree.MouseWheel += (c, ee) => (c as Control).Zoom_MouseWheel(ee);
            this.ScanAndUpdate();
            this.bnSelectWorkTree.Focus();
        }

        WorktreeInfo Worktree
        {
            get => this._worktree;
            set
            {
                if (this._worktree != value)
                {
                    this._worktree = value;
                    this.tbWorkTree.Text = value?.ToString();
                    this.lbWorktree.Text = value?.Branch.CanonicalName;
                    OnWorktreePathChanged?.Invoke(this.WorktreePath);
                }
            }
        }

        internal string WorktreePath => this.Worktree == null
            ? null
            : RepoGit.GetWorktreePath(this.RepoWorkingCopyPath, this.Worktree.Name);

        internal string RepoWorkingCopyPath
        {
            get => this.tbRepo.Text;
            set => this.tbRepo.Text = value;
        }

        internal event Action<string> OnWorktreePathChanged;

        void Report(string text) => this.OnReport?.Invoke(text);
        internal Action<string> OnReport;
        WorktreeInfo _worktree;

        void ui_update()
        {
            var repo = RepoBase.GetRepo(this.RepoWorkingCopyPath, this.Report) as IRepoGit;
            bool mainEN = repo != null && this.Worktree != null;
            this.bnAdd.Enabled = mainEN && repo.GetWorkTrees().FindIndex(a => a.Branch.CmpName == this.Worktree.Branch.CmpName) == -1;
            this.bnRemove.Enabled = mainEN && !this.bnAdd.Enabled;
            this.tbWorkTree.ReadOnly = !mainEN || this.bnRemove.Enabled;
            this.bnExplore.Enabled = this.bnRemove.Enabled;
            this.bnSelectWorkTree.Enabled = repo != null;
            this.bnSelectBranch.Enabled = repo != null;
        }

        void bnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.SelectedPath = UTL.GetExistingFolder(this.RepoWorkingCopyPath);
            if (d.ShowDialog() == DialogResult.OK)
            {
                this.RepoWorkingCopyPath = d.SelectedPath;
                this.ScanAndUpdate();
            }
        }

        void bnExplore_Click(object sender, EventArgs e)
        {
            var ini = SettingsIni.GetIni();
            var section = ini.GetSection(SettingsIni.Key.ExploreWorktreeSection);

            if (section.Bool(SettingsIni.Key.CloseDuplicates, true))
                UTL.CloseDuplicatedExplorerWindows();

            var s = Path.GetDirectoryName(this.RepoWorkingCopyPath);
            UTL.Explore(Path.Combine(s, this.Worktree.Name),
                section.Bool(SettingsIni.Key.Select, true),
                section.Bool(SettingsIni.Key.TrySingleWindow, true));
        }

        void cbRepo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ScanAndUpdate();
        }

        void ScanAndUpdate()
        {
            try
            {
                if (Directory.Exists(this.RepoWorkingCopyPath))
                {
                    var s = Path.Combine(this.RepoWorkingCopyPath, ".git");
                    if (File.Exists(s))
                    {
                        var text = File.ReadAllText(s);
                        var m = Regex.Match(text, "gitdir: (.+)/.git/.+");
                        if (m.Groups.Count == 2)
                        {
                            string workTreePath = this.RepoWorkingCopyPath;
                            this.RepoWorkingCopyPath = m.Groups[1].Value.Replace('/', '\\');

                            var repo = RepoBase.GetRepo(this.RepoWorkingCopyPath, this.Report) as IRepoGit;
                            string workTreeName = Path.GetFileName(workTreePath);
                            this.Worktree = repo.GetWorkTrees().FirstOrDefault(a => a.Name == workTreeName);
                        }
                    }
                }
                this.ui_update();
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        void bnRefresh_Click(object sender, EventArgs e)
        {
            this.ScanAndUpdate();
        }

        void bnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.AddWorkTree();
                this.ScanAndUpdate();
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        void AddWorkTree()
        {
            string tmpFile = Path.GetTempFileName();
            tmpFile = Path.ChangeExtension(tmpFile, "bat");

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                var sb = new StringBuilder();

                var name = this.Worktree.Name;
                var disc = Path.GetPathRoot(this.RepoWorkingCopyPath);
                disc = disc.Substring(0, disc.Length - 1);
                sb.AppendLine($"{disc}");
                sb.AppendLine($"cd {this.RepoWorkingCopyPath}");
                sb.AppendLine($"git worktree add ..\\{name} {this.Worktree.Branch.FriendlyName}");
                sb.AppendLine($"");

                var localBranch = this.Worktree.Branch.FriendlyName.Replace("origin/", string.Empty);
                var path = Path.Combine(Path.GetDirectoryName(this.RepoWorkingCopyPath), name);
                sb.AppendLine($"cd {path}");
                sb.AppendLine($"git branch {localBranch} -t {this.Worktree.Branch.FriendlyName}");
                sb.AppendLine($"");

                path = Path.Combine(Path.GetDirectoryName(this.RepoWorkingCopyPath), name);
                sb.AppendLine($"cd {path}");
                sb.AppendLine($"git switch {localBranch}");
                sb.AppendLine($"");

                sb.AppendLine($"PAUSE");
                var text = sb.ToString();
                File.WriteAllText(tmpFile, text);

                var p = Process.Start("cmd.exe", $"/C {tmpFile}");
                p.WaitForExit();
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
            finally
            {
                File.Delete(tmpFile);
                Cursor.Current = Cursors.Default;
            }
        }

        void bnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show($"Remove worktree '{this.Worktree.Name}' ?", "Remove work tree", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3) == DialogResult.Yes)
                {
                    this.RemoveWorkTree();
                    this.ScanAndUpdate();
                }
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        void RemoveWorkTree()
        {
            string tmpFile = Path.GetTempFileName();
            tmpFile = Path.ChangeExtension(tmpFile, "bat");

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                var sb = new StringBuilder();

                var name = this.Worktree.Name;
                var disc = Path.GetPathRoot(this.RepoWorkingCopyPath);
                disc = disc.Substring(0, disc.Length - 1);
                sb.AppendLine($"{disc}");
                sb.AppendLine($"cd {this.RepoWorkingCopyPath}");
                sb.AppendLine($"git worktree remove {this.Worktree.Name}");
                sb.AppendLine($"");

                sb.AppendLine($"PAUSE");
                var text = sb.ToString();
                File.WriteAllText(tmpFile, text);

                var p = Process.Start("cmd.exe", $"/C {tmpFile}");
                p.WaitForExit();
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
            finally
            {
                File.Delete(tmpFile);
                Cursor.Current = Cursors.Default;
            }

            this.Worktree = null;
        }

        void bnSelectWorkTree_Click(object sender, EventArgs e)
        {
            try
            {
                var repo = RepoBase.GetRepo(this.RepoWorkingCopyPath, this.Report) as IRepoGit;
                var o = this.SelectItem(repo.GetWorkTrees(), null) as WorktreeInfo;
                if (o != null)
                    this.Worktree = o;
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
            this.ui_update();
        }

        void bnSelectBranch_Click(object sender, EventArgs e)
        {
            try
            {
                var repo = RepoBase.GetRepo(this.RepoWorkingCopyPath, this.Report) as IRepoGit;
                var o = this.SelectItem(repo.GetBranches().DistinctBy(a => a.CmpName), branch =>
                {
                    var s = branch.ToString();
                    if (s.StartsWith("refs/"))
                    {
                        s = s.Replace("refs/heads/", string.Empty);
                        s = s.Replace("refs/remotes/", string.Empty);
                    }
                    return s;
                }) as BranchInfo;

                if (o != null)
                {
                    var wts = repo.GetWorkTrees();
                    var w = wts.FirstOrDefault(a => a.Branch.CmpName == o.CmpName);
                    if (w != null)
                        this.Worktree = w;
                    else
                        this.Worktree = new WorktreeInfo(o);
                }
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
            this.ui_update();
        }

        object SelectItem(IEnumerable<object> items, Func<object, string> displayText)
        {
            object ret = null;
            var f = new ListForm();
            f.DisplayText = displayText;
            f.SetItems(items);
            if (f.ShowDialog() == DialogResult.OK)
            {
                ret = f.SelectedItem;
            }
            return ret;
        }

        void bnExploreRepo_Click(object sender, EventArgs e)
        {
            var ini = SettingsIni.GetIni();
            var section = ini.GetSection(SettingsIni.Key.ExploreRepoSection);

            if (section.Bool(SettingsIni.Key.CloseDuplicates, true))
                UTL.CloseDuplicatedExplorerWindows();

            UTL.Explore(this.tbRepo.Text,
                section.Bool(SettingsIni.Key.Select, true),
                section.Bool(SettingsIni.Key.TrySingleWindow, true));
        }

        void bnSelectRepo_Click(object sender, EventArgs e)
        {
            try
            {
                if (UTL.Browse(this.tbRepo.Text, out var s, TEXT.SelectGitRepo))
                {
                    this.RepoWorkingCopyPath = RepoBase.GetWorkingCopyFolder(s);
                    this.Worktree = null;
                    this.ScanAndUpdate();
                }
            }
            catch (Exception ee)
            {
                this.Report(ee.Message);
            }
        }

        internal string SaveUI()
        {
            return ControlEx.SaveUI(this.ParentForm, this.tbRepo, this.tbWorkTree);
        }

        private void tbWorkTree_TextChanged(object sender, EventArgs e)
        {
            if (this.Worktree != null)
            {
                this.Worktree.Name = this.tbWorkTree.Text;
            }
        }
    }
}