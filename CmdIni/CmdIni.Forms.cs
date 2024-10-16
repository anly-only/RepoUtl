using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace cmd_ini.Forms
{
#if false
    
    cmm = new CmdIni.CmdIniMenu(cm); // in Form constructor

    void cm_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
        cmm.cm_Opening(sender, GetCommands());
    }

    void cm_Closed(object sender, ToolStripDropDownClosedEventArgs e)
    {
        cmm.cm_Closed(form, Report);
    }

#endif

    public class CmdIniMenu
    {
        ContextMenuStrip cm;

        public CmdIniMenu(ContextMenuStrip cm)
        {
            this.cm = cm;
        }

        public void cm_Opening(object sender, IEnumerable<Cmd> commands)
        {
            this.Clear();
            bool prevSeparator = this.cm.Items.Count == 0 || this.cm.Items[this.cm.Items.Count - 1] is ToolStripSeparator;
            foreach (Cmd cmd in commands)
            {
                if (cmd.IsSeparator)
                {
                    if (!prevSeparator)
                        this.cm.Items.Add(new ToolStripSeparator() { Tag = cmd });
                    prevSeparator = true;
                }
                else
                {
                    var c = this.cm.Items.Add(cmd.Name);
                    c.Enabled = cmd.Enabled;
                    c.Tag = cmd; // remember command in menu Tag
                    c.MouseDown += this.menu_MouseDown;
                    prevSeparator = false;
                }
            }
        }

        void menu_MouseDown(object sender, MouseEventArgs e)
        {
            var c = (ToolStripItem)sender;
            var cmd = (Cmd)c.Tag;
            this.cm.Tag = cmd; // remember command in Tag of ContextMenu
        }

        // Execute by Close menu
        public void cm_Closed(Cmd cmd, IEnumerable<string> selected, Control parent, Action<string> report)
        {
            CmdExec.Execute(cmd, selected, parent, report);
            this.Clear();
        }

        public void Clear()
        {
            for (int i = this.cm.Items.Count - 1; i >= 0; i--)
            {
                ToolStripItem item = this.cm.Items[i];
                if (item.Tag is Cmd) // remove only my items
                {
                    item.MouseDown -= this.menu_MouseDown;
                    this.cm.Items.RemoveAt(i);
                }
            }
            this.cm.Tag = null;
        }
    }
}
