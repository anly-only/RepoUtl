using forms_ex;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RepoUtl
{
    public partial class WorkTreeForm : Form
    {
        public WorkTreeForm()
        {
            InitializeComponent();
        }

        void WorkTreeForm_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.WorktreesFormUI.IsEmpty())
                ControlEx.ReadUI(this.GetControls(true), Properties.Settings.Default.WorktreesFormUI);
        }

        void WorkTreeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.WorktreesFormUI = uc.SaveUI();
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            this.ProcessKeyPreviewHandler(ref m, () => Close());
            return base.ProcessKeyPreview(ref m);
        }
    }
}
