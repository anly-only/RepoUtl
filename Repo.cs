using System;
using System.IO;

namespace RepoUtl
{
    enum RepoKind
    {
        None, Svn, Git
    }

    interface IRepo
    {
        Action<string> Report { get; }
        string WorkingCopy { get; }

        void EnumChanges(Action<RepoItem> back);
        void IgnoreUnversioned();
        void CopyOriginalTo(RepoItem item, string target);
    }

    interface RepoItem
    {
        string Path { get; }
        ItemStatus Status { get; }
    }

    internal enum ItemStatus
    {
        None,
        Modified,
        Unversioned
    }

    static class RepoBase
    {
        internal static IRepo GetRepo(string path, Action<string> Report)
        {
            IRepo repo = null;

            var kind = GetRepoKind(path, out string workingCopy);
            switch (kind)
            {
                case RepoKind.Svn:
                    repo = new RepoSvn(workingCopy, Report);
                    break;
                case RepoKind.Git:
                    repo = new RepoGit(workingCopy, Report);
                    break;
            }

            return repo;
        }

        internal static string GetWorkingCopyFolder(string path)
        {
            GetRepoKind(path, out string wc);
            return wc;
        }

        internal static RepoKind GetRepoKind(string path, out string wc)
        {
            RepoKind kind = RepoKind.None;

            string svnRepo = GetWorkingCopyFolderSvn(path);
            string gitRepo = GetWorkingCopyFolderGit(path);

            if (!svnRepo.IsEmpty() && !gitRepo.IsEmpty())
            {
                kind = svnRepo.Length > gitRepo.Length ? RepoKind.Svn : RepoKind.Git;
            }
            else if (!svnRepo.IsEmpty())
            {
                kind = RepoKind.Svn;
            }
            else if (!gitRepo.IsEmpty())
            {
                kind = RepoKind.Git;
            }

            wc = kind == RepoKind.Git ? gitRepo : kind == RepoKind.Svn ? svnRepo : string.Empty;

            return kind;
        }

        static string GetWorkingCopyFolderSvn(string path)
        {
            return GetWorkingCopyFolder(path, RepoSvn.IsWorkingCopyFolder);
        }

        static string GetWorkingCopyFolderGit(string path)
        {
            return GetWorkingCopyFolder(path, RepoGit.IsWorkingCopyFolder);
        }

        static string GetWorkingCopyFolder(string parentFolder, Func<string, bool> IsWorkingCopyFolder)
        {
            string repoFolder = string.Empty;

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = parentFolder;
            if (folder.IndexOf(appData, StringComparison.InvariantCulture) == 0)
            {
                // ignore AppData folder
            }
            else
            {
                int level = 0;
                while (repoFolder.IsEmpty() && !folder.IsEmpty())
                {
                    if (IsWorkingCopyFolder(folder))
                        repoFolder = folder;
                    else if (++level < int.MaxValue)
                        folder = Path.GetDirectoryName(folder);
                    else
                        break;
                }
            }

            return repoFolder;
        }
    }
}
