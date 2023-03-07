using csutl;
using forms_ex;
using LibGit2Sharp;
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
            InitializeComponent();
        }

        void WorkTreeUC_Load(object sender, EventArgs e)
        {
            tbRepo.MouseWheel += (c, ee) => (c as Control).Zoom_MouseWheel(ee);
            tbWorkTree.MouseWheel += (c, ee) => (c as Control).Zoom_MouseWheel(ee);
            ScanAndUpdate();
            bnSelectWorkTree.Focus();
        }

        WorktreeInfo Worktree
        {
            get => _worktree;
            set
            {
                if (_worktree != value)
                {
                    _worktree = value;
                    tbWorkTree.Text = value?.ToString();
                    lbWorktree.Text = value?.Branch.CanonicalName;
                    OnWorktreePathChanged?.Invoke(WorktreePath);
                }
            }
        }

        internal string WorktreePath => Worktree == null
            ? null
            : RepoGit.GetWorktreePath(RepoWorkingCopyPath, Worktree.Name);

        internal string RepoWorkingCopyPath
        {
            get => tbRepo.Text;
            set => tbRepo.Text = value;
        }

        internal event Action<string> OnWorktreePathChanged;

        void Report(string text) => OnReport?.Invoke(text);
        internal Action<string> OnReport;
        WorktreeInfo _worktree;

        void ui_update()
        {
            var repo = RepoBase.GetRepo(RepoWorkingCopyPath, Report) as IRepoGit;
            bool mainEN = repo != null && Worktree != null;
            bnAdd.Enabled = mainEN && repo.GetWorkTrees().FindIndex(a => a.Branch.CmpName == Worktree.Branch.CmpName) == -1;
            bnRemove.Enabled = mainEN && !bnAdd.Enabled;
            tbWorkTree.ReadOnly = !mainEN || bnRemove.Enabled;
            bnExplore.Enabled = bnRemove.Enabled;
            bnSelectWorkTree.Enabled = repo != null;
            bnSelectBranch.Enabled = repo != null;
        }

        void bnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.SelectedPath = UTL.GetExistingFolder(RepoWorkingCopyPath);
            if (d.ShowDialog() == DialogResult.OK)
            {
                RepoWorkingCopyPath = d.SelectedPath;
                ScanAndUpdate();
            }
        }

        void bnExplore_Click(object sender, EventArgs e)
        {
            var ini = SettingsIni.GetIni();
            var section = ini.GetSection(SettingsIni.Key.ExploreWorktreeSection);

            if (section.Bool(SettingsIni.Key.CloseDuplicates, true))
                UTL.CloseDuplicatedExplorerWindows();

            var s = Path.GetDirectoryName(RepoWorkingCopyPath);
            UTL.Explore(Path.Combine(s, Worktree.Name),
                section.Bool(SettingsIni.Key.Select, true),
                section.Bool(SettingsIni.Key.TrySingleWindow, true));
        }

        void cbRepo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScanAndUpdate();
        }

        void ScanAndUpdate()
        {
            try
            {
                if (Directory.Exists(RepoWorkingCopyPath))
                {
                    var s = Path.Combine(RepoWorkingCopyPath, ".git");
                    if (File.Exists(s))
                    {
                        var text = File.ReadAllText(s);
                        var m = Regex.Match(text, "gitdir: (.+)/.git/.+");
                        if (m.Groups.Count == 2)
                        {
                            string workTreePath = RepoWorkingCopyPath;
                            RepoWorkingCopyPath = m.Groups[1].Value.Replace('/', '\\');

                            var repo = RepoBase.GetRepo(RepoWorkingCopyPath, Report) as IRepoGit;
                            string workTreeName = Path.GetFileName(workTreePath);
                            Worktree = repo.GetWorkTrees().FirstOrDefault(a => a.Name == workTreeName);
                        }
                    }
                }
                ui_update();
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        void bnRefresh_Click(object sender, EventArgs e)
        {
            ScanAndUpdate();
        }

        void bnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                AddWorkTree();
                ScanAndUpdate();
            }
            catch (Exception ee)
            {
                Report(ee.Message);
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

                var name = Worktree.Name;
                var disc = Path.GetPathRoot(RepoWorkingCopyPath);
                disc = disc.Substring(0, disc.Length - 1);
                sb.AppendLine($"{disc}");
                sb.AppendLine($"cd {RepoWorkingCopyPath}");
                sb.AppendLine($"git worktree add ..\\{name} {Worktree.Branch.FriendlyName}");
                sb.AppendLine($"");

                var localBranch = Worktree.Branch.FriendlyName.Replace("origin/", string.Empty);
                var path = Path.Combine(Path.GetDirectoryName(RepoWorkingCopyPath), name);
                sb.AppendLine($"cd {path}");
                sb.AppendLine($"git branch {localBranch} -t {Worktree.Branch.FriendlyName}");
                sb.AppendLine($"");

                path = Path.Combine(Path.GetDirectoryName(RepoWorkingCopyPath), name);
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
                Report(ee.Message);
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
                if (MessageBox.Show($"Remove worktree '{Worktree.Name}' ?", "Remove work tree", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3) == DialogResult.Yes)
                {
                    RemoveWorkTree();
                    ScanAndUpdate();
                }
            }
            catch (Exception ee)
            {
                Report(ee.Message);
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

                var name = Worktree.Name;
                var disc = Path.GetPathRoot(RepoWorkingCopyPath);
                disc = disc.Substring(0, disc.Length - 1);
                sb.AppendLine($"{disc}");
                sb.AppendLine($"cd {RepoWorkingCopyPath}");
                sb.AppendLine($"git worktree remove {Worktree.Name}");
                sb.AppendLine($"");

                sb.AppendLine($"PAUSE");
                var text = sb.ToString();
                File.WriteAllText(tmpFile, text);

                var p = Process.Start("cmd.exe", $"/C {tmpFile}");
                p.WaitForExit();
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
            finally
            {
                File.Delete(tmpFile);
                Cursor.Current = Cursors.Default;
            }

            Worktree = null;
        }

        void bnSelectWorkTree_Click(object sender, EventArgs e)
        {
            try
            {
                var repo = RepoBase.GetRepo(RepoWorkingCopyPath, Report) as IRepoGit;
                var o = SelectItem(repo.GetWorkTrees(), null) as WorktreeInfo;
                if (o != null)
                    Worktree = o;
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
            ui_update();
        }

        void bnSelectBranch_Click(object sender, EventArgs e)
        {
            try
            {
                var repo = RepoBase.GetRepo(RepoWorkingCopyPath, Report) as IRepoGit;
                var o = SelectItem(repo.GetBranches().DistinctBy(a => a.CmpName), branch =>
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
                        Worktree = w;
                    else
                        Worktree = new WorktreeInfo(o);
                }
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
            ui_update();
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

            UTL.Explore(tbRepo.Text,
                section.Bool(SettingsIni.Key.Select, true),
                section.Bool(SettingsIni.Key.TrySingleWindow, true));
        }

        void bnSelectRepo_Click(object sender, EventArgs e)
        {
            try
            {
                if (UTL.Browse(tbRepo.Text, out var s, TEXT.SelectGitRepo))
                {
                    RepoWorkingCopyPath = RepoBase.GetWorkingCopyFolder(s);
                    Worktree = null;
                    ScanAndUpdate();
                }
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        internal string SaveUI()
        {
            return ControlEx.SaveUI(ParentForm, tbRepo, tbWorkTree);
        }

        private void tbWorkTree_TextChanged(object sender, EventArgs e)
        {
            if (Worktree != null)
            {
                Worktree.Name = tbWorkTree.Text;
            }
        }
    }
}