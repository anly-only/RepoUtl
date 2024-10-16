using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace cmd_ini
{
    public static class CmdExec
    {
        public static void Execute(Cmd cmd, IEnumerable<string> selected, Control parent, Action<string> report)
        {
            int selCount = selected.Count();
            var args = cmd.Ini.GetArguments(cmd.Args).ToArray();
            if (args.Length == 0)
            {
                Execute(cmd, parent, report);
            }
            else if (args.Length == 1)
            {
                if (selCount == 0)
                    report?.Invoke("Ignored: nothing selected");

                foreach (string path in selected)
                {
                    cmd.Ini.Macros.SetNumberedMacros(Enumerable.Repeat(path, 1));
                    Execute(cmd, parent, report);
                    cmd.Ini.Macros.RemoveNumberedMacros();
                }
            }
            else
            {
                if (report != null && args.Length != selCount)
                    report.Invoke("The selected count doesn't match to the command arguments count");

                cmd.Ini.Macros.SetNumberedMacros(selected);
                Execute(cmd, parent, report);
                cmd.Ini.Macros.RemoveNumberedMacros();
            }
        }

        static void Execute(Cmd cmd, Control parent, Action<string> report)
        {
            try
            {
                cmd = cmd.Clone();
                cmd.File = cmd.Ini.ApplyMacros(cmd.File);
                cmd.Args = cmd.Ini.ApplyMacros(cmd.Args);

                Process p = new Process();
                p.StartInfo.FileName = cmd.File;
                p.StartInfo.Arguments = cmd.Args;
                p.StartInfo.UseShellExecute = true;
                p.Start();

                if (cmd.WaitForExit)
                {
                    if (parent != null)
                        parent.Enabled = false;
                    p.WaitForExit();
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
