namespace RepoUtl
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.bnChanges = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbRepo = new System.Windows.Forms.ComboBox();
            this.cbCopyOriginal = new System.Windows.Forms.CheckBox();
            this.tbReport = new System.Windows.Forms.TextBox();
            this.cmReport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bnIgnoreUnversioned = new System.Windows.Forms.Button();
            this.bnMergeChanges = new System.Windows.Forms.Button();
            this.cmCopy = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bnCopyChanges = new System.Windows.Forms.ToolStripMenuItem();
            this.bnExplore = new System.Windows.Forms.Button();
            this.cm = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.aaaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bnBrowse = new System.Windows.Forms.Button();
            this.cbPostfix = new System.Windows.Forms.ComboBox();
            this.bnWorkTree = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.cmReport.SuspendLayout();
            this.cmCopy.SuspendLayout();
            this.cm.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnChanges
            // 
            this.bnChanges.Location = new System.Drawing.Point(3, 93);
            this.bnChanges.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bnChanges.Name = "bnChanges";
            this.bnChanges.Size = new System.Drawing.Size(159, 42);
            this.bnChanges.TabIndex = 4;
            this.bnChanges.Text = "Show modified";
            this.bnChanges.UseVisualStyleBackColor = true;
            this.bnChanges.Click += this.Status_Click;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 165F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 165F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 137F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 137F));
            this.tableLayoutPanel1.Controls.Add(this.cbRepo, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbCopyOriginal, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.tbReport, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.bnIgnoreUnversioned, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.bnChanges, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.bnMergeChanges, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.bnExplore, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.bnBrowse, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbPostfix, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.bnWorkTree, 3, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(608, 509);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // cbRepo
            // 
            this.cbRepo.AllowDrop = true;
            this.cbRepo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.tableLayoutPanel1.SetColumnSpan(this.cbRepo, 5);
            this.cbRepo.FormattingEnabled = true;
            this.cbRepo.Location = new System.Drawing.Point(3, 4);
            this.cbRepo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbRepo.Name = "cbRepo";
            this.cbRepo.Size = new System.Drawing.Size(602, 31);
            this.cbRepo.TabIndex = 0;
            this.cbRepo.SelectedIndexChanged += this.cbRepo_SelectedIndexChanged;
            this.cbRepo.TextUpdate += this.tbLocalPath_TextUpdate;
            this.cbRepo.DragDrop += this.tbLocalPath_DragDrop;
            this.cbRepo.DragEnter += this.tbLocalPath_DragEnter;
            // 
            // cbCopyOriginal
            // 
            this.cbCopyOriginal.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cbCopyOriginal.AutoSize = true;
            this.cbCopyOriginal.Checked = true;
            this.cbCopyOriginal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCopyOriginal.Location = new System.Drawing.Point(214, 143);
            this.cbCopyOriginal.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbCopyOriginal.Name = "cbCopyOriginal";
            this.cbCopyOriginal.Size = new System.Drawing.Size(67, 27);
            this.cbCopyOriginal.TabIndex = 7;
            this.cbCopyOriginal.Text = "base";
            this.cbCopyOriginal.UseVisualStyleBackColor = true;
            // 
            // tbReport
            // 
            this.tbReport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.tbReport.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel1.SetColumnSpan(this.tbReport, 5);
            this.tbReport.ContextMenuStrip = this.cmReport;
            this.tbReport.Location = new System.Drawing.Point(3, 193);
            this.tbReport.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbReport.Multiline = true;
            this.tbReport.Name = "tbReport";
            this.tbReport.ReadOnly = true;
            this.tbReport.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbReport.Size = new System.Drawing.Size(602, 312);
            this.tbReport.TabIndex = 0;
            this.tbReport.TabStop = false;
            // 
            // cmReport
            // 
            this.cmReport.ImageScalingSize = new System.Drawing.Size(19, 19);
            this.cmReport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.clearToolStripMenuItem });
            this.cmReport.Name = "cmReport";
            this.cmReport.Size = new System.Drawing.Size(120, 32);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(119, 28);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += this.clearToolStripMenuItem_Click;
            // 
            // bnIgnoreUnversioned
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.bnIgnoreUnversioned, 2);
            this.bnIgnoreUnversioned.Location = new System.Drawing.Point(3, 43);
            this.bnIgnoreUnversioned.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bnIgnoreUnversioned.Name = "bnIgnoreUnversioned";
            this.bnIgnoreUnversioned.Size = new System.Drawing.Size(324, 42);
            this.bnIgnoreUnversioned.TabIndex = 1;
            this.bnIgnoreUnversioned.Text = "Svn: ignore unversioned folders";
            this.bnIgnoreUnversioned.UseVisualStyleBackColor = true;
            this.bnIgnoreUnversioned.Click += this.bnIgnoreUnversioned_Click;
            // 
            // bnMergeChanges
            // 
            this.bnMergeChanges.ContextMenuStrip = this.cmCopy;
            this.bnMergeChanges.Location = new System.Drawing.Point(168, 93);
            this.bnMergeChanges.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bnMergeChanges.Name = "bnMergeChanges";
            this.bnMergeChanges.Size = new System.Drawing.Size(159, 42);
            this.bnMergeChanges.TabIndex = 5;
            this.bnMergeChanges.Text = "Copy modified";
            this.bnMergeChanges.UseVisualStyleBackColor = true;
            this.bnMergeChanges.Click += this.bnMergeChanges_Click;
            // 
            // cmCopy
            // 
            this.cmCopy.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmCopy.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.bnCopyChanges });
            this.cmCopy.Name = "cm";
            this.cmCopy.Size = new System.Drawing.Size(312, 32);
            this.cmCopy.Opening += this.CmCopy_Opening;
            // 
            // bnCopyChanges
            // 
            this.bnCopyChanges.Name = "bnCopyChanges";
            this.bnCopyChanges.Size = new System.Drawing.Size(311, 28);
            this.bnCopyChanges.Text = "Copy modified into new folder";
            this.bnCopyChanges.Click += this.bnCopyChanges_Click;
            // 
            // bnExplore
            // 
            this.bnExplore.ContextMenuStrip = this.cm;
            this.bnExplore.Location = new System.Drawing.Point(474, 43);
            this.bnExplore.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bnExplore.Name = "bnExplore";
            this.bnExplore.Size = new System.Drawing.Size(130, 42);
            this.bnExplore.TabIndex = 3;
            this.bnExplore.Text = "Explore...";
            this.bnExplore.UseVisualStyleBackColor = true;
            this.bnExplore.Click += this.bnExplore_Click;
            // 
            // cm
            // 
            this.cm.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.aaaToolStripMenuItem });
            this.cm.Name = "cm";
            this.cm.Size = new System.Drawing.Size(167, 32);
            this.cm.Closed += this.cm_Closed;
            this.cm.Opening += this.cm_Opening;
            // 
            // aaaToolStripMenuItem
            // 
            this.aaaToolStripMenuItem.Name = "aaaToolStripMenuItem";
            this.aaaToolStripMenuItem.Size = new System.Drawing.Size(166, 28);
            this.aaaToolStripMenuItem.Text = "--dummy--";
            // 
            // bnBrowse
            // 
            this.bnBrowse.Location = new System.Drawing.Point(337, 43);
            this.bnBrowse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bnBrowse.Name = "bnBrowse";
            this.bnBrowse.Size = new System.Drawing.Size(129, 42);
            this.bnBrowse.TabIndex = 2;
            this.bnBrowse.Text = "Select...";
            this.bnBrowse.UseVisualStyleBackColor = true;
            this.bnBrowse.Click += this.bnBrowse_Click;
            // 
            // cbPostfix
            // 
            this.cbPostfix.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel1.SetColumnSpan(this.cbPostfix, 2);
            this.cbPostfix.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.cbPostfix.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbPostfix.FormattingEnabled = true;
            this.cbPostfix.Location = new System.Drawing.Point(337, 98);
            this.cbPostfix.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbPostfix.Name = "cbPostfix";
            this.cbPostfix.Size = new System.Drawing.Size(268, 32);
            this.cbPostfix.TabIndex = 6;
            this.cbPostfix.SelectedIndexChanged += this.cbPostfix_SelectedIndexChanged;
            this.cbPostfix.TextChanged += this.cbPostfix_TextChanged;
            this.cbPostfix.Leave += this.cbPostfix_Leave;
            // 
            // bnWorkTree
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.bnWorkTree, 2);
            this.bnWorkTree.Location = new System.Drawing.Point(337, 143);
            this.bnWorkTree.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bnWorkTree.Name = "bnWorkTree";
            this.bnWorkTree.Size = new System.Drawing.Size(268, 42);
            this.bnWorkTree.TabIndex = 8;
            this.bnWorkTree.Text = "Git: worktrees...";
            this.bnWorkTree.UseVisualStyleBackColor = true;
            this.bnWorkTree.Click += this.bnWorkTree_Click;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 509);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Repo utilities";
            this.FormClosing += this.Form1_FormClosing;
            this.Load += this.Form1_Load;
            this.Shown += this.Form1_Shown;
            this.SizeChanged += this.Form1_SizeChanged;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.cmReport.ResumeLayout(false);
            this.cmCopy.ResumeLayout(false);
            this.cm.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Button bnChanges;
        private System.Windows.Forms.ContextMenuStrip cmReport;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.Button bnIgnoreUnversioned;
        private System.Windows.Forms.Button bnBrowse;
        private System.Windows.Forms.Button bnExplore;
        private System.Windows.Forms.Button bnMergeChanges;
        private System.Windows.Forms.ComboBox cbRepo;
        private System.Windows.Forms.ComboBox cbPostfix;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox cbCopyOriginal;
        private System.Windows.Forms.Button bnWorkTree;
        private System.Windows.Forms.TextBox tbReport;
        private System.Windows.Forms.ContextMenuStrip cm;
        private System.Windows.Forms.ToolStripMenuItem aaaToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmCopy;
        private System.Windows.Forms.ToolStripMenuItem bnCopyChanges;
    }
}

