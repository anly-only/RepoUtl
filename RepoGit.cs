using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RepoUtl
{
    interface IRepoGit : IRepo
    {
        List<BranchInfo> GetBranches();
        List<WorktreeInfo> GetWorkTrees();
    }

    internal class RepoGit : IRepoGit
    {
        public Action<string> Report { get; }
        public string WorkingCopy { get; }

        internal RepoGit(string workingCopy, Action<string> Report)
        {
            WorkingCopy = workingCopy;
            this.Report = Report;
        }

        public void CopyOriginalTo(RepoItem item, string target)
        {
            try
            {
                using (var repo = new Repository(WorkingCopy))
                {
                    var relative = item.Path.Substring(WorkingCopy.Length + 1);
                    var treeEntry = repo.Head.Tip[relative];

                    if (treeEntry != null)
                    {
                        Debug.Assert(treeEntry.TargetType == TreeEntryTargetType.Blob);
                        var blob = (Blob)treeEntry.Target;

                        var contentStream = blob.GetContentStream();
                        Debug.Assert(blob.Size == contentStream.Length);

                        using (var r = new StreamReader(contentStream))
                        {
                            using (var w = File.OpenWrite(target))
                            {
                                r.BaseStream.CopyTo(w);
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        public void EnumChanges(Action<RepoItem> back)
        {
            using (var repo = new Repository(WorkingCopy))
            {
                StatusOptions arg = new StatusOptions()
                {
                    Show = StatusShowOption.IndexAndWorkDir,
                    IncludeIgnored = false,
                    IncludeUntracked = true
                };
                RepositoryStatus statuses = repo.RetrieveStatus(arg);
                foreach (var e in statuses)
                {
                    var item = new RepoItemGit(this, e);
                    back.Invoke(item);
                }
            }
        }

        public void IgnoreUnversioned()
        {
            Report.Invoke("Not implemented.");
        }

        internal static bool IsWorkingCopyFolder(string folder)
        {
            bool yes = !folder.IsEmpty()
                && (Directory.Exists(Path.Combine(folder, ".git")) || File.Exists(Path.Combine(folder, ".git")))
                && Repository.IsValid(folder);

            return yes;
        }

        public List<BranchInfo> GetBranches()
        {
            var ret = new List<BranchInfo>();
            using (var repo = new Repository(WorkingCopy))
            {
                foreach (var a in repo.Branches)
                {
                    var branch = new BranchInfo(a);
                    ret.Add(branch);
                }
            }
            return ret;
        }

        public List<WorktreeInfo> GetWorkTrees()
        {
            var ret = new List<WorktreeInfo>();
            using (var repo = new Repository(WorkingCopy))
            {
                foreach (var w in repo.Worktrees)
                {
                    try
                    {
                        var ww = new WorktreeInfo(w);
                        if (ww.Branch != null)
                            ret.Add(ww);
                    }
                    catch (Exception ee)
                    {
                        Report?.Invoke($"{w.Name}: {ee.Message}");
                    }
                }
            }
            return ret;
        }

        internal static string GetWorktreePath(string repoWorkingCopyPath, string worktrreName)
        {
            return Path.Combine(Path.GetDirectoryName(repoWorkingCopyPath), worktrreName);
        }

        class RepoItemGit : RepoItem
        {
            RepoGit repo;
            StatusEntry status;

            public RepoItemGit(RepoGit repo, StatusEntry e)
            {
                this.repo = repo;
                status = e;
            }

            public string Path => System.IO.Path.Combine(repo.WorkingCopy, status.FilePath);

            public ItemStatus Status =>
                status.State.HasFlag(FileStatus.ModifiedInWorkdir) || status.State.HasFlag(FileStatus.ModifiedInIndex)
                    ? ItemStatus.Modified
                    : status.State.HasFlag(FileStatus.NewInWorkdir)
                        ? ItemStatus.Unversioned
                        : ItemStatus.None;
        }
    }


    class BranchInfo
    {
        internal BranchInfo(Branch b)
        {
            CanonicalName = b.CanonicalName;
            FriendlyName = b.FriendlyName;
            IsRemote = b.IsRemote;
        }
        internal string CanonicalName { get; private set; }
        internal string FriendlyName { get; private set; }
        internal bool IsRemote { get; private set; }
        public override string ToString() => FriendlyName;
    }

    class WorktreeInfo
    {
        internal WorktreeInfo(BranchInfo b)
        {
            Branch = b;
            Name = Path.GetFileName(b.FriendlyName);
        }
        internal WorktreeInfo(Worktree w)
        {
            Name = w.Name;
            var h = w.WorktreeRepository.Head;
            if (h is Branch b)
                Branch = new BranchInfo(b);
        }
        internal string Name { get; set; }
        internal BranchInfo Branch { get; private set; }

        public override string ToString() => Name;
    }
}
