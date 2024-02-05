using SharpSvn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RepoUtl
{
    interface IRepoSvn : IRepo
    {
    }

    class RepoSvn : IRepoSvn
    {
        public Action<string> Report { get; }
        public string WorkingCopy { get; }

        internal RepoSvn(string workingCopy, Action<string> Report)
        {
            WorkingCopy = workingCopy;
            this.Report = Report;
        }

        public void CopyOriginalTo(RepoItem item, string target)
        {
            try
            {
                using (SvnClient client = new SvnClient())
                {
                    SvnTarget a = SvnTarget.FromString(item.Path);
                    client.Export(a, target, new SvnExportArgs() { Revision = SvnRevision.Committed, Overwrite = true });
                }
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        public void EnumChanges(Action<RepoItem> back)
        {
            using (SvnClient client = new SvnClient())
            {
                var arg = new SvnStatusArgs
                {
                    Depth = SvnDepth.Infinity,
                    //RetrieveRemoteStatus = true,
                    RetrieveAllEntries = true
                };

                client.Status(WorkingCopy, arg, (s, e) =>
                {
                    if (e.NodeKind == SvnNodeKind.File)
                    {
                        var item = new RepoItemSvn(e);
                        if (item.Status != ItemStatus.None)
                        {
                            back.Invoke(item);
                        } 
                    }
                });
            }
        }

        public void IgnoreUnversioned()
        {
            try
            {
                using (SvnClient svn = new SvnClient())
                {
                    Report("ignore in " + WorkingCopy + " ...");
                    ignoreFiles(svn, WorkingCopy);
                    Report("OK");
                }
            }
            catch (Exception ee)
            {
                Report(ee.Message);
            }
        }

        void ignoreFiles(SvnClient svn, string parent_folder)
        {
            string[] folders = Directory.GetDirectories(parent_folder);
            List<string> check = new List<string>();
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                if (name != ".svn")
                {
                    try
                    {
                        string val;
                        if (svn.GetProperty(SvnTarget.FromString(folder), SvnPropertyNames.SvnIgnore, out val))
                        {
                            check.Add(folder);
                        }
                        else
                        {
                            string s = val;
                        }
                    }
                    catch (SvnUnversionedNodeException e)
                    {
                        try
                        {
                            string s = e.Message;
                            string val;
                            if (svn.GetProperty(SvnTarget.FromString(parent_folder), SvnPropertyNames.SvnIgnore, out val))
                            {
                                List<string> ignore = val == null ? new List<string>() : new List<string>(val.Split('\r', '\n'));
                                if (ignore.IndexOf(name) == -1)
                                {
                                    if (val == null)
                                        val = string.Empty;
                                    val += name + "\r\n";
                                    svn.SetProperty(parent_folder, SvnPropertyNames.SvnIgnore, val);
                                    Report?.Invoke(folder);
                                }
                                else
                                {
                                    // already ignored
                                }
                            }
                        }
                        catch (Exception ee)
                        {
                            Report?.Invoke("set ignore fails: " + ee.Message);
                        }
                    }
                }
            }

            foreach (string folder in check)
            {
                ignoreFiles(svn, folder);
            }
        }

        internal static bool IsWorkingCopyFolder(string folder)
        {
            bool yes = !folder.IsEmpty()
                && Directory.Exists(Path.Combine(folder, ".svn"))
                && SvnTools.IsManagedPath(folder);
            return yes;
        }

        class RepoItemSvn : RepoItem
        {
            SvnStatusEventArgs status;

            public RepoItemSvn(SvnStatusEventArgs e)
            {
                status = e;
            }

            public string Path => status.FullPath;

            public ItemStatus Status => status.Modified ? ItemStatus.Modified : !status.Versioned ? ItemStatus.Unversioned : ItemStatus.None;
        }
    }
}
