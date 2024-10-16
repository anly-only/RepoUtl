using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RepoUtl
{
    static class UTL
    {
        const int SW_RESTORE = 9;
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);


        internal static string GetExistingFolder(string path)
        {
            while (!path.IsEmpty() && !Directory.Exists(path))
                path = Path.GetDirectoryName(path);
            return path;
        }

        internal static void Explore(string path, bool select, bool trySingleWindow)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (!File.Exists(path) && !Directory.Exists(path))
                path = UTL.GetExistingFolder(path);

            if (select)
            {
                if (trySingleWindow)
                    KillExplorerWindow(Path.GetDirectoryName(path));

                Process.Start("explorer.exe", "/select, \"" + path + "\"");
            }
            else
            {
                if (trySingleWindow)
                    Process.Start(path);
                else
                    Process.Start("explorer.exe", "/open, \"" + path + "\"");
            }
        }

        static void KillExplorerWindow(string path)
        {
            SHDocVw.ShellWindows ww = new SHDocVw.ShellWindows();
            foreach (var item in ww.Cast<SHDocVw.InternetExplorer>().ToArray())
            {
                try
                {
                    var u = new Uri(item.LocationURL);
                    if (u.LocalPath == path)
                        item.Quit();
                }
                catch
                {
                }
            }
        }

        internal static void KillProcess(string name, string tittle)
        {
            GetProcessesByName(name, tittle)
                .Foreach(p => p.Kill());
        }

        internal static IEnumerable<Process> GetProcessesByName(string name, string tittle)
        {
            return Process.GetProcessesByName(name).Where(a => !a.HasExited && a.ProcessName == name && a.MainWindowTitle == tittle);
        }

        // note, tittle of several processes can be same
        internal static bool BringProcessToFront(string name, string tittle)
        {
            bool ok = false;
            var p = GetProcessesByName(name, tittle).FirstOrDefault();
            if (p != null)
                ok = BringProcessToFront(p);
            return ok;
        }

        internal static bool BringProcessToFront(Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            if (IsIconic(handle))
                ShowWindow(handle, SW_RESTORE);
            return SetForegroundWindow(handle);
        }

        internal static void CloseDuplicatedExplorerWindows()
        {
            try
            {
                SHDocVw.ShellWindows ww = new SHDocVw.ShellWindows();
                var list = ww.Cast<SHDocVw.InternetExplorer>()
                    .Reverse() // first new windows, then old windows
                    .ToList();

                for (int i = 0; i < list.Count - 1; i++)
                {
                    var p1 = list[i];
                    for (int j = i + 1; j < list.Count; j++)
                    {
                        var p2 = list[j];
                        if (p1.LocationURL == p2.LocationURL)
                        {
                            p2.Quit(); // kill old window
                            list.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        internal static bool Browse(string initialFolder, out string selectedFolder, string description = null)
        {
            bool ok = false;
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.Description = description;
            d.SelectedPath = UTL.GetExistingFolder(initialFolder);
            if (d.ShowDialog() == DialogResult.OK)
            {
                selectedFolder = d.SelectedPath;
                ok = true;
            }
            else
            {
                selectedFolder = null;
            }
            return ok;
        }

        internal static void Report(TextBox tb, string text)
        {
            Action a = () =>
            {
                tb.Text += text + "\r\n";
                tb.SelectionStart = tb.Text.Length;
                tb.SelectionLength = 0;
                tb.ScrollToCaret();
                Application.DoEvents();
            };

            if (tb.InvokeRequired)
                tb.Invoke(a);
            else
                a.Invoke();
        }

        internal static bool IsEmpty(this string s) => string.IsNullOrEmpty(s);
    }
}
