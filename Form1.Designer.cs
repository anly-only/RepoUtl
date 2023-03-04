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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            this.bnChanges.Location = new System.Drawing.Point(3, 68);
            this.bnChanges.Name = "bnChanges";
            this.bnChanges.Size = new System.Drawing.Size(142, 29);
            this.bnChanges.TabIndex = 4;
            this.bnChanges.Text = "Show modified";
            this.bnChanges.UseVisualStyleBackColor = true;
            this.bnChanges.Click += new System.EventHandler(this.Status_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 148F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 148F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.tableLayoutPanel1.Controls.Add(this.cbRepo, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbCopyOriginal, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.tbReport, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.bnIgnoreUnversioned, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.bnChanges, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.bnMergeChanges, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.bnExplore, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.bnBrowse, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbPostfix, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.bnWorkTree, 2, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(540, 354);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // cbRepo
            // 
            this.cbRepo.AllowDrop = true;
            this.cbRepo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.cbRepo, 4);
            this.cbRepo.FormattingEnabled = true;
            this.cbRepo.Location = new System.Drawing.Point(3, 3);
            this.cbRepo.Name = "cbRepo";
            this.cbRepo.Size = new System.Drawing.Size(534, 24);
            this.cbRepo.TabIndex = 0;
            this.cbRepo.SelectedIndexChanged += new System.EventHandler(this.cbRepo_SelectedIndexChanged);
            this.cbRepo.TextUpdate += new System.EventHandler(this.tbLocalPath_TextUpdate);
            this.cbRepo.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbLocalPath_DragDrop);
            this.cbRepo.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbLocalPath_DragEnter);
            // 
            // cbCopyOriginal
            // 
            this.cbCopyOriginal.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cbCopyOriginal.AutoSize = true;
            this.cbCopyOriginal.Checked = true;
            this.cbCopyOriginal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCopyOriginal.Location = new System.Drawing.Point(174, 103);
            this.cbCopyOriginal.Name = "cbCopyOriginal";
            this.cbCopyOriginal.Size = new System.Drawing.Size(95, 20);
            this.cbCopyOriginal.TabIndex = 7;
            this.cbCopyOriginal.Text = "base";
            this.cbCopyOriginal.UseVisualStyleBackColor = true;
            // 
            // tbReport
            // 
            this.tbReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.tbReport, 4);
            this.tbReport.ContextMenuStrip = this.cmReport;
            this.tbReport.Location = new System.Drawing.Point(3, 138);
            this.tbReport.Multiline = true;
            this.tbReport.Name = "tbReport";
            this.tbReport.ReadOnly = true;
            this.tbReport.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbReport.Size = new System.Drawing.Size(534, 213);
            this.tbReport.TabIndex = 0;
            this.tbReport.TabStop = false;
            // 
            // cmReport
            // 
            this.cmReport.ImageScalingSize = new System.Drawing.Size(19, 19);
            this.cmReport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
            this.cmReport.Name = "cmReport";
            this.cmReport.Size = new System.Drawing.Size(113, 28);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(112, 24);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // bnIgnoreUnversioned
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.bnIgnoreUnversioned, 2);
            this.bnIgnoreUnversioned.Location = new System.Drawing.Point(3, 33);
            this.bnIgnoreUnversioned.Name = "bnIgnoreUnversioned";
            this.bnIgnoreUnversioned.Size = new System.Drawing.Size(290, 29);
            this.bnIgnoreUnversioned.TabIndex = 1;
            this.bnIgnoreUnversioned.Text = "Svn: ignore unversioned folders";
            this.bnIgnoreUnversioned.UseVisualStyleBackColor = true;
            this.bnIgnoreUnversioned.Click += new System.EventHandler(this.bnIgnoreUnversioned_Click);
            // 
            // bnCopyChanges
            // 
            this.bnMergeChanges.ContextMenuStrip = this.cmCopy;
            this.bnMergeChanges.Location = new System.Drawing.Point(151, 68);
            this.bnMergeChanges.Name = "bnCopyChanges";
            this.bnMergeChanges.Size = new System.Drawing.Size(142, 29);
            this.bnMergeChanges.TabIndex = 5;
            this.bnMergeChanges.Text = "Copy modified";
            this.bnMergeChanges.UseVisualStyleBackColor = true;
            this.bnMergeChanges.Click += new System.EventHandler(this.bnMergeChanges_Click);
            // 
            // cmCopy
            // 
            this.cmCopy.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmCopy.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bnCopyChanges});
            this.cmCopy.Name = "cm";
            this.cmCopy.Size = new System.Drawing.Size(218, 56);
            this.cmCopy.Opening += this.CmCopy_Opening;
            // 
            // bnCopyNew
            // 
            this.bnCopyChanges.Name = "bnCopyNew";
            this.bnCopyChanges.Size = new System.Drawing.Size(217, 24);
            this.bnCopyChanges.Text = "Copy modified into new folder";
            this.bnCopyChanges.Click += new System.EventHandler(this.bnCopyChanges_Click);
            // 
            // bnExplore
            // 
            this.bnExplore.ContextMenuStrip = this.cm;
            this.bnExplore.Location = new System.Drawing.Point(421, 33);
            this.bnExplore.Name = "bnExplore";
            this.bnExplore.Size = new System.Drawing.Size(116, 29);
            this.bnExplore.TabIndex = 3;
            this.bnExplore.Text = "Explore...";
            this.bnExplore.UseVisualStyleBackColor = true;
            this.bnExplore.Click += new System.EventHandler(this.bnExplore_Click);
            // 
            // cm
            // 
            this.cm.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aaaToolStripMenuItem});
            this.cm.Name = "cm";
            this.cm.Size = new System.Drawing.Size(103, 28);
            this.cm.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.cm_Closed);
            this.cm.Opening += new System.ComponentModel.CancelEventHandler(this.cm_Opening);
            // 
            // aaaToolStripMenuItem
            // 
            this.aaaToolStripMenuItem.Name = "aaaToolStripMenuItem";
            this.aaaToolStripMenuItem.Size = new System.Drawing.Size(102, 24);
            this.aaaToolStripMenuItem.Text = "aaa";
            // 
            // bnBrowse
            // 
            this.bnBrowse.Location = new System.Drawing.Point(299, 33);
            this.bnBrowse.Name = "bnBrowse";
            this.bnBrowse.Size = new System.Drawing.Size(115, 29);
            this.bnBrowse.TabIndex = 2;
            this.bnBrowse.Text = "Select...";
            this.bnBrowse.UseVisualStyleBackColor = true;
            this.bnBrowse.Click += new System.EventHandler(this.bnBrowse_Click);
            // 
            // cbPostfix
            // 
            this.cbPostfix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.cbPostfix, 2);
            this.cbPostfix.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPostfix.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbPostfix.FormattingEnabled = true;
            this.cbPostfix.Location = new System.Drawing.Point(299, 68);
            this.cbPostfix.Name = "cbPostfix";
            this.cbPostfix.Size = new System.Drawing.Size(238, 28);
            this.cbPostfix.TabIndex = 6;
            this.cbPostfix.SelectedIndexChanged += new System.EventHandler(this.cbPostfix_SelectedIndexChanged);
            this.cbPostfix.TextChanged += new System.EventHandler(this.cbPostfix_TextChanged);
            this.cbPostfix.Leave += new System.EventHandler(this.cbPostfix_Leave);
            // 
            // bnWorkTree
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.bnWorkTree, 2);
            this.bnWorkTree.Location = new System.Drawing.Point(299, 103);
            this.bnWorkTree.Name = "bnWorkTree";
            this.bnWorkTree.Size = new System.Drawing.Size(238, 29);
            this.bnWorkTree.TabIndex = 8;
            this.bnWorkTree.Text = "Git: worktrees...";
            this.bnWorkTree.UseVisualStyleBackColor = true;
            this.bnWorkTree.Click += new System.EventHandler(this.bnWorkTree_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 354);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Repo utilities";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
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

