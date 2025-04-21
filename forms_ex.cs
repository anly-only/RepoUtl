using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace forms_ex
{
    static class ControlEx
    {
        private const int WM_SETREDRAW = 0x000B;

        public static void SuspendUpdate(this Control control)
        {
            Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        public static void ResumeUpdate(this Control control)
        {
            // Create a C "true" boolean as an IntPtr
            IntPtr wparam = new IntPtr(1);
            Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);

            control.Invalidate();
        }

        public static void EnsureVisible(this Form form)
        {
            Rectangle rc = Screen.FromControl(form).Bounds;
            Point p = form.Location;
            p.X = Math.Min(p.X, rc.Right - form.Width);
            p.Y = Math.Min(p.Y, rc.Bottom - form.Height);
            p.X = Math.Max(p.X, rc.Left);
            p.Y = Math.Max(p.Y, rc.Top);
            form.Location = p;
        }

        public static void SetFontSize(this Control c, double size)
        {
            if (size > 0)
            {
                try
                {
                    c.Font = new Font(c.Font.FontFamily, (float)size, FontStyle.Regular, c.Font.Unit);
                }
                catch (Exception)
                {
                }
            }
        }

        // example:  tree.MouseWheel += (c, e) => (c as Control).Zoom_MouseWheel(e);
        public static void Zoom_MouseWheel(this Control c, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                if (e.Delta != 0)
                {
                    try
                    {
                        float prevSize = c.Font.Size;
                        float newSize = prevSize + (e.Delta > 0 ? 1 : -1);
                        newSize = Math.Max(5, newSize);
                        newSize = Math.Min(30, newSize);
                        if (newSize != prevSize)
                        {
                            c.SetFontSize(newSize);
                            c.Invalidate();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        public static void HideEmptySeparators(this ToolStrip menu)
        {
            ToolStripItem lastSep = null;
            bool empty = true;
            foreach (ToolStripItem item in menu.Items)
            {
                if (item is ToolStripSeparator)
                {
                    item.Visible = !empty;
                    empty = true;
                    if (item.Visible)
                        lastSep = item;
                }
                else
                {
                    if (item.Visible)
                    {
                        lastSep = null;
                        empty = false;
                    }
                }
            }

            if (lastSep != null)
                lastSep.Visible = false;
        }

        /// <summary>
        /// Move item to begin of list and select
        /// </summary>
        internal static void SelectItem(this ComboBox cb, object item, Func<object, string> compareText = null)
        {
            var text = compareText != null ? compareText(item) : item.ToString();

            if (string.IsNullOrEmpty(text))
            {
                cb.SelectedIndex = -1;
            }
            else
            {
                for (int i = cb.Items.Count - 1; i >= 0; i--)
                {
                    var x = compareText != null ? compareText(cb.Items[i]) : cb.Items[i].ToString();
                    if (x == text)
                        cb.Items.RemoveAt(i);
                }
                cb.Items.Insert(0, item);
                cb.SelectedIndex = 0;
            }
        }

        internal static void SelectLines(this TextBoxBase tb, int CodeLine, int CodeLinesCount)
        {
            if (CodeLine != -1 && CodeLinesCount != 0)
            {
                int start = tb.GetFirstCharIndexFromLine(CodeLine);
                int end = tb.GetFirstCharIndexFromLine(CodeLine + CodeLinesCount);
                if (start != -1 && end != -1)
                {
                    tb.SelectionStart = start;
                    tb.SelectionLength = 0;
                    tb.ScrollToCaret();
                    tb.SelectionLength = end - start;
                }
            }
            else
            {
                tb.SelectionLength = 0;
            }
        }

        // call from: protected override bool ProcessKeyPreview(ref Message m)
        internal static void ProcessKeyPreviewHandler(this Form f, ref Message m, Action escape, Action enter = null)
        {
            if (m.Msg == /*WM_CHAR*/ 0x0102) // do not use WM_KEYDOWN to awoid beep by KEYUP
            {
                if (((Keys)m.WParam == Keys.Escape))
                {
                    escape?.Invoke();
                }
                else if ((Keys)m.WParam == Keys.Enter)
                {
                    enter?.Invoke();
                }
            }
        }

        #region Controls UI
        internal static List<Control> GetControls(this Control parent, bool includeParent)
        {
            var ret = new List<Control>();
            parent.EnumControls(includeParent, c => ret.Add(c));
            return ret;
        }

        internal static void EnumControls(this Control parent, bool includeParent, Action<Control> act)
        {
            if (includeParent)
                act(parent);

            foreach (Control item in parent.Controls)
                item.EnumControls(true, act);
        }

        internal enum CP // Control Property
        {
            All, //All properties listed below if exists

            FontSize,            // Control
            Form_Size,           // Form
            Form_Location,       // Form
            ListView_Colums      // ListView
        }

        internal static string SaveUI(params Control[] controls)
        {
            return SaveUI(controls.Select(a => (a, CP.All)).ToArray());
        }

        internal static string SaveUI(params (Control c, CP cp)[] controls)
        {
            var list = controls.ToList();
            var list2 = controls.ToList().Select(a => a.c).ToList();
            return SaveUI(list2, (c, cp) => list.Any(a => a.c == c && (a.cp == cp || a.cp == CP.All)));
        }

        internal static string SaveUI(IEnumerable<Control> controls, Func<Control, CP, bool> IsSave = null)
        {
            var sb = new StringBuilder();

            Action<Control, string, string> Add = (control, prop, value) => sb.Append($"{control.Name}.{prop}={value};\n");

            foreach (var c in controls)
            {
                if (IsSave == null || IsSave(c, CP.FontSize))
                    Add(c, CP.FontSize.ToString(), c.Font.Size.ToString());

                if (c is Form form)
                {
                    if ((IsSave == null || IsSave(form, CP.Form_Size)))
                        Add(form, CP.Form_Size.ToString(), form.Size.ToString());
                    if ((IsSave == null || IsSave(form, CP.Form_Location)))
                        Add(form, CP.Form_Location.ToString(), form.Location.ToString());
                }

                if (c is ListView lv && (IsSave == null || IsSave(lv, CP.ListView_Colums)))
                    Add(lv, CP.ListView_Colums.ToString(), lv.SaveColumns());
            }

            var s = sb.ToString();
            return s;
        }

        internal static void ReadUI(string UI, params Control[] controls) => ReadUI(controls, UI);

        internal static void ReadUI(IEnumerable<Control> controls, string UI)
        {
            try
            {
                var lines = UI.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                foreach (var line in lines)
                {
                    var m = Regex.Match(line, @"(\w+)\.(\w+)=(.+)");
                    if (m.Success)
                    {
                        var name = m.Groups[1].Value;
                        var c = controls.First(a => a.Name == name);
                        CP prop = (CP)Enum.Parse(typeof(CP), m.Groups[2].Value);
                        var value = m.Groups[3].Value;
                        switch (prop)
                        {
                            case CP.FontSize:
                            {
                                var f = float.Parse(value);
                                c.SetFontSize(f);
                                break;
                            }
                            case CP.Form_Size:
                            {
                                var m2 = Regex.Match(value, @"Width=(\d+).+Height=(\d+)");
                                var form = (Form)c;
                                c.Width = int.Parse(m2.Groups[1].Value);
                                c.Height = int.Parse(m2.Groups[2].Value);
                                break;
                            }
                            case CP.Form_Location:
                            {
                                var m2 = Regex.Match(value, @"X=(-?\d+).+Y=(-?\d+)");
                                var form = (Form)c;
                                form.Location = new Point(int.Parse(m2.Groups[1].Value), int.Parse(m2.Groups[2].Value));
                                form.EnsureVisible();
                                break;
                            }
                            case CP.ListView_Colums:
                            {
                                var lv = (ListView)c;
                                lv.ReadColumns(value);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Debug.Fail(ee.Message);
            }
        }

        internal static string SaveColumns(this ListView lv)
        {
            string s = string.Empty;
            foreach (ColumnHeader c in lv.Columns)
            {
                if (s != string.Empty)
                    s += ";";
                s += c.Width;
            }
            return s;
        }

        internal static void ReadColumns(this ListView lv, string s)
        {
            string[] ss = s.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < ss.Length; i++)
            {
                if (i < lv.Columns.Count)
                {
                    lv.Columns[i].Width = int.Parse(ss[i]);
                }
            }
        }

        #endregion Controls UI
    }
}

