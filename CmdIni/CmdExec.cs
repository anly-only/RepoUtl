using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cmd_ini
{
    public static class CmdExec
    {
        public static async void Execute(Cmd cmd, IEnumerable<string> selected, Control parent, Action<string> report)
        {
            int selCount = selected.Count();
            int pathsCount = cmd.Ini.ArgPathsCount(cmd.Args);
            if (pathsCount == 0)
            {
                await Execute(cmd, parent, report);
            }
            else if (pathsCount == 1)
            {
                if (selCount == 0)
                    report?.Invoke("Ignored: nothing selected");

                foreach (string path in selected)
                {
                    cmd.Ini.Macros.SetMacro("path", path);
                    await Execute(cmd, parent, report);
                }
            }
            else if (pathsCount == 2 && selected.Count() == 2)
            {
                cmd.Ini.Macros.SetMacro("path", selected.First());
                cmd.Ini.Macros.SetMacro("path2", selected.Skip(1).First());
                await Execute(cmd, parent, report);
            }
            else
            {
                if (report != null && pathsCount != selCount)
                    report.Invoke("The selected count doesn't match to the command arguments count");
            }
        }

        static async Task Execute(Cmd cmd, Control parent, Action<string> report)
        {
            try
            {
                cmd = cmd.Clone();
                cmd.File = cmd.Ini.ApplyMacros(cmd.File);
                cmd.Args = cmd.Ini.ApplyMacros(cmd.Args);

                var p = new Process();
                p.StartInfo.FileName = cmd.File;
                p.StartInfo.Arguments = cmd.Args;
                p.StartInfo.UseShellExecute = true;
                p.Start();

                if (cmd.WaitForExit)
                {
                    if (parent != null)
                        parent.Enabled = false;
                    // wait without blocking the UI thread
                    await p.WaitForExitAsync();
                }
            }
            catch (Exception ee)
            {
                report?.Invoke(ee.Message);
            }
            finally
            {
                if (parent != null)
                    parent.Enabled = true;
            }
        }
    }
}
