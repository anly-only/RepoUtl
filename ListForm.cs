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
    public partial class ListForm : Form
    {
        List<object> items = new List<object>();
        List<object> view = new List<object>();
        Func<object, string> displayTextImpl = DisplayTextDefaultImpl;

        public ListForm()
        {
            InitializeComponent();
            list.MouseWheel += (c, e) => (c as Control).Zoom_MouseWheel(e);
            tbFilter.MouseWheel += (c, e) => (c as Control).Zoom_MouseWheel(e);
        }

        void ListForm_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.ListFormUI.IsEmpty())
                ControlEx.ReadUI(this.GetControls(true), Properties.Settings.Default.ListFormUI);

        }

        void ListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.ListFormUI = ControlEx.SaveUI(this, tbFilter, list);
        }


        /// <summary>
        /// This converter is used for display and filter texts.
        /// </summary>
        public Func<object, string> DisplayText
        {
            get => displayTextImpl;
            set => displayTextImpl = value ?? DisplayTextDefaultImpl;
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
            updateView();
        }

        static string DisplayTextDefaultImpl(object a) => a.ToString();

        void updateView()
        {
            var s = tbFilter.Text;
            view = items.Where(a => DisplayText(a).Contains(s) || s.IsEmpty()).ToList();
            list.VirtualListSize = view.Count;
            list.Invalidate();
        }

        void tbFilter_TextChanged(object sender, EventArgs e)
        {
            updateView();
        }

        void list_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var s = DisplayText(view[e.ItemIndex]);
            e.Item = new ListViewItem(s);
        }

        void list_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            var x = list.SelectedIndices;
            if (x.Count != 0)
                SelectedItem = view[x[0]];
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            bool processed = false;
            this.ProcessKeyPreviewHandler(ref m,
                () =>
                {
                    if (tbFilter.Focused)
                    {
                        if (tbFilter.Text.Length == 0)
                            Close();
                        else
                            tbFilter.Clear();
                    }
                    else
                    {
                        tbFilter.Focus();
                    }
                    processed = true;
                },
                () =>
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    processed = true;
                });

            if (!processed)
                processed = base.ProcessKeyPreview(ref m);

            return processed;
        }
    }
}
