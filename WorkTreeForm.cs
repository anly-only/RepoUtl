using forms_ex;
using System;
using System.Windows.Forms;

namespace RepoUtl
{
    public partial class WorkTreeForm : Form
    {
        public WorkTreeForm()
        {
            this.InitializeComponent();
        }

        void WorkTreeForm_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.WorktreesFormUI.IsEmpty())
                ControlEx.ReadUI(this.GetControls(true), Properties.Settings.Default.WorktreesFormUI);
        }

        void WorkTreeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.WorktreesFormUI = this.uc.SaveUI();
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            this.ProcessKeyPreviewHandler(ref m, () => this.Close());
            return base.ProcessKeyPreview(ref m);
        }
    }
}
