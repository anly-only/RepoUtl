using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);


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


        public static void KillExplorerWindow(string _path)
        {
            string path = Path.GetFileName(_path);
            foreach (var p in Process.GetProcessesByName("explorer"))
            {
                IntPtr h = p.MainWindowHandle;
                if (h == IntPtr.Zero || !IsWindowVisible(h))
                    continue;

                var sb = new StringBuilder(1024);
                GetWindowText(h, sb, sb.Capacity);

                string title = sb.ToString();
                int ix = title.IndexOf(" — ");
                if (ix != -1)
                    title = title.Substring(0, ix).Trim();

                if (string.Compare(title, path, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    p.Kill();
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

        public static void CloseDuplicatedExplorerWindows()
        {
            IEnumerable<(Process Process, string Path)> windows = GetExplorerWindows();

            // first new windows, then old windows
            List<(Process Process, string Path)> list = windows.OrderByDescending(w => w.Process.StartTime).ToList();

            for (int i = 0; i < list.Count - 1; i++)
            {
                var w1 = list[i];
                for (int j = i + 1; j < list.Count; j++)
                {
                    var w2 = list[j];
                    if (string.Equals(w1.Path, w2.Path, StringComparison.OrdinalIgnoreCase))
                    {
                        w2.Process.Kill();
                        list.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        private static IEnumerable<(Process Process, string Path)> GetExplorerWindows()
        {
            var result = new List<(Process, string)>();

            EnumWindows((hWnd, lParam) =>
            {
                if (!IsWindowVisible(hWnd))
                    return true;

                uint pid;
                GetWindowThreadProcessId(hWnd, out pid);

                var p = Process.GetProcessById((int)pid);
                if (!string.Equals(p.ProcessName, "explorer", StringComparison.OrdinalIgnoreCase))
                    return true;

                var sb = new StringBuilder(1024);
                GetWindowText(hWnd, sb, sb.Capacity);
                string title = sb.ToString();

                // Explorer показывает путь в заголовке
                if (!string.IsNullOrEmpty(title))
                {
                    int ix = title.IndexOf(" — ");
                    if (ix != -1)
                        title = title.Substring(0, ix).Trim();
                    result.Add((p, title));
                }

                return true;
            }, IntPtr.Zero);

            return result;
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
