using forms_ex;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace RepoUtl
{
    public partial class ListForm : Form
    {
        List<object> items = new List<object>();
        List<object> view = new List<object>();
        Func<object, string> displayTextImpl = DisplayTextDefaultImpl;

        public ListForm()
        {
            this.InitializeComponent();
            this.list.MouseWheel += (c, e) => (c as Control).Zoom_MouseWheel(e);
            this.tbFilter.MouseWheel += (c, e) => (c as Control).Zoom_MouseWheel(e);
        }

        void ListForm_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.ListFormUI.IsEmpty())
                ControlEx.ReadUI(this.GetControls(true), Properties.Settings.Default.ListFormUI);

        }

        void ListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.ListFormUI = ControlEx.SaveUI(this, this.tbFilter, this.list);
        }


        /// <summary>
        /// This converter is used for display and filter texts.
        /// </summary>
        public Func<object, string> DisplayText
        {
            get => this.displayTextImpl;
            set => this.displayTextImpl = value ?? DisplayTextDefaultImpl;
        }

        /// <summary>
        /// selected object in the list
        /// </summary>
        public object SelectedItem { get; private set; }

        /// <summary>
        /// Set items in the list
        /// </summary>
        public void SetItems(IEnumerable<object> items)
        {
            this.items = items.ToList();
            this.updateView();
        }

        static string DisplayTextDefaultImpl(object a) => a.ToString();

        void updateView()
        {
            string s = this.tbFilter.Text;

            this.view = s.IsEmpty()
                ? this.items.ToList()
                : this.items.Where(a => this.DisplayText(a).Contains(s, StringComparison.InvariantCultureIgnoreCase)).ToList();

            this.list.VirtualListSize = this.view.Count;
            this.list.Invalidate();
        }

        void tbFilter_TextChanged(object sender, EventArgs e)
        {
            this.updateView();
        }

        void list_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var s = this.DisplayText(this.view[e.ItemIndex]);
            e.Item = new ListViewItem(s);
        }

        void list_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            var x = this.list.SelectedIndices;
            if (x.Count != 0)
                this.SelectedItem = this.view[x[0]];
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            bool processed = false;
            this.ProcessKeyPreviewHandler(ref m,
                () =>
                {
                    if (this.tbFilter.Focused)
                    {
                        if (this.tbFilter.Text.Length == 0)
                            this.Close();
                        else
                            this.tbFilter.Clear();
                    }
                    else
                    {
                        this.tbFilter.Focus();
                    }
                    processed = true;
                },
                () =>
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    processed = true;
                });

            if (!processed)
                processed = base.ProcessKeyPreview(ref m);

            return processed;
        }
    }
}
